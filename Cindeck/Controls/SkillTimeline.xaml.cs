using Cindeck.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cindeck.Controls
{
    /// <summary>
    /// SkillTimeline.xaml の相互作用ロジック
    /// </summary>
    public partial class SkillTimeline : UserControl
    {
        private Unit m_unit;

        public SkillTimeline()
        {
            InitializeComponent();
        }

        private string GetSkillSummary(OwnedIdol idol)
        {
            if(idol==null)
            {
                return "編成されていません\r\n\r\n";
            }
            var name= string.Format("{0}[{1}]{2}({3})\r\n", string.IsNullOrEmpty(idol.Label) ? "" : string.Format("[{0}]", idol.Label),
                idol.Rarity.ToLocalizedString(), idol.Name,
                idol.Skill == null ? "特技なし" : string.Format("{0}Lv{1}", idol.Skill.Name, idol.SkillLevel));
            var skillEffect = idol.Skill == null ? "\r\n" : GetSkillEffect(idol.Skill)+"\r\n";
            var skillDetail = idol.Skill == null ? "" : GetSkillDetails(idol.Skill, idol.SkillLevel);
            return name + skillEffect + skillDetail;
        }

        private string GetSkillType(ISkill skill)
        {
            if(skill is Skill.ComboBonus)
            {
                return "コンボボーナス";
            }
            if (skill is Skill.ComboContinuation)
            {
                return "コンボ継続";
            }
            if (skill is Skill.DamageGuard)
            {
                return "ダメージガード";
            }
            if (skill is Skill.JudgeEnhancement)
            {
                return "判定強化";
            }
            if (skill is Skill.Revival)
            {
                return "ライフ回復";
            }
            if (skill is Skill.ScoreBonus)
            {
                return "スコアボーナス";
            }
            return null;
        }

        private string GetSkillEffect(ISkill skill)
        {
            if (skill is Skill.ComboBonus)
            {
                var s = skill as Skill.ComboBonus;
                return string.Format("コンボボーナスが{0:P0}アップ",s.Rate);
            }
            if (skill is Skill.ComboContinuation)
            {
                var s = skill as Skill.ComboContinuation;
                return string.Format("{0}でもコンボが継続する", s.Targets.ToLocalizedString());
            }
            if (skill is Skill.DamageGuard)
            {
                var s = skill as Skill.DamageGuard;
                return string.Format("ライフが減少しなくなる");
            }
            if (skill is Skill.JudgeEnhancement)
            {
                var s = skill as Skill.JudgeEnhancement;
                return string.Format("{0}をPERFECTにする", s.Targets.ToLocalizedString());
            }
            if (skill is Skill.Revival)
            {
                var s = skill as Skill.Revival;
                return string.Format("PERFECTでライフが{0}回復", s.Amount);
            }
            if (skill is Skill.ScoreBonus)
            {
                var s = skill as Skill.ScoreBonus;
                return string.Format("{0}のスコアが{1:P0}アップ", s.Targets.ToLocalizedString(), s.Rate);
            }
            return null;
        }

        private string GetSkillDetails(ISkill skill,int lv)
        {
            return string.Format("{0}秒ごとに{1:P1}の確率で発動(持続時間{2:F1}秒)", skill.Interval,skill.EstimateProbability(lv),skill.EstimateDuration(lv));
        }

        private void LoadSlots()
        {
            Slot1Label.Content = GetSkillSummary(m_unit.GetValueOrDefault(x => x.Slot1));
            Slot2Label.Content = GetSkillSummary(m_unit.GetValueOrDefault(x => x.Slot2));
            Slot3Label.Content = GetSkillSummary(m_unit.GetValueOrDefault(x => x.Slot3));
            Slot4Label.Content = GetSkillSummary(m_unit.GetValueOrDefault(x => x.Slot4));
            Slot5Label.Content = GetSkillSummary(m_unit.GetValueOrDefault(x => x.Slot5));

            DrawTimeline(Slot1Canvas, m_unit.GetValueOrDefault(x => x.Slot1));
            DrawTimeline(Slot2Canvas, m_unit.GetValueOrDefault(x => x.Slot2));
            DrawTimeline(Slot3Canvas, m_unit.GetValueOrDefault(x => x.Slot3));
            DrawTimeline(Slot4Canvas, m_unit.GetValueOrDefault(x => x.Slot4));
            DrawTimeline(Slot5Canvas, m_unit.GetValueOrDefault(x => x.Slot5));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawTimeline(Slot1Canvas, m_unit.GetValueOrDefault(x => x.Slot1));
            DrawTimeline(Slot2Canvas, m_unit.GetValueOrDefault(x => x.Slot2));
            DrawTimeline(Slot3Canvas, m_unit.GetValueOrDefault(x => x.Slot3));
            DrawTimeline(Slot4Canvas, m_unit.GetValueOrDefault(x => x.Slot4));
            DrawTimeline(Slot5Canvas, m_unit.GetValueOrDefault(x => x.Slot5));
            base.OnRender(drawingContext);
        }

        private void DrawTimeline(Canvas c, OwnedIdol idol)
        {
            c.Children.Clear();
            if (idol==null||idol.Skill==null||c.ActualHeight==0)
            {
                return;
            }
            double interval = idol.Skill.Interval;
            var duration = idol.Skill.EstimateDuration(idol.SkillLevel);
            var playTime = 120;
            var triggerCount = playTime / interval;
            var totalWidth = c.ActualWidth;
            var skillWidth = (duration / playTime) * totalWidth;
            var intervalWidth = (interval / playTime) * totalWidth;
            var brush = GetBrushBySkill(idol.Skill);

            for(int i=1;i<triggerCount;i++)
            {
                double left = i * intervalWidth;
                double right = left + skillWidth;
                var rect = new Rectangle { Fill = brush,
                    Width = right > c.ActualWidth?skillWidth-(right - c.ActualWidth): skillWidth,
                    Height =c.ActualHeight, Opacity=1 };
                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, 0);
                c.Children.Add(rect);
            }
        }

        private Brush GetBrushBySkill(ISkill skill)
        {
            if (skill is Skill.ComboBonus)
            {
                return Resources["ComboBonusColorBrush"] as Brush;
            }
            if (skill is Skill.ComboContinuation)
            {
                return Resources["ComboContinuationColorBrush"] as Brush;
            }
            if (skill is Skill.DamageGuard)
            {
                return Resources["DamageGuardColorBrush"] as Brush;
            }
            if (skill is Skill.JudgeEnhancement)
            {
                return Resources["JudgeEnhancementColorBrush"] as Brush;
            }
            if (skill is Skill.Revival)
            {
                return  Resources["RevivalColorBrush"] as Brush;
            }
            if (skill is Skill.ScoreBonus)
            {
                return Resources["ScoreBonusColorBrush"] as Brush;
            }
            return null;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.Property==DataContextProperty)
            {
                if(e.NewValue!=null && e.NewValue is Unit)
                {
                    m_unit = e.NewValue as Unit;
                }

                if (e.OldValue != null)
                {
                    (e.OldValue as INotifyPropertyChanged).PropertyChanged -= SkillTimeline_PropertyChanged;
                    var u = e.OldValue as Unit;
                    UnregisterProperptyChanged(u.Slot1);
                    UnregisterProperptyChanged(u.Slot2);
                    UnregisterProperptyChanged(u.Slot3);
                    UnregisterProperptyChanged(u.Slot4);
                    UnregisterProperptyChanged(u.Slot5);
                }

                if (e.NewValue != null)
                {
                    (e.NewValue as INotifyPropertyChanged).PropertyChanged += SkillTimeline_PropertyChanged;
                    var u = e.NewValue as Unit;
                    RegisterProperptyChanged(u.Slot1);
                    RegisterProperptyChanged(u.Slot2);
                    RegisterProperptyChanged(u.Slot3);
                    RegisterProperptyChanged(u.Slot4);
                    RegisterProperptyChanged(u.Slot5);
                }

                LoadSlots();
            }
            base.OnPropertyChanged(e);
        }

        private void SkillTimeline_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LoadSlots();
            if(sender is Unit)
            {
                var u = sender as Unit;

                UnregisterProperptyChanged(u.Slot1);
                UnregisterProperptyChanged(u.Slot2);
                UnregisterProperptyChanged(u.Slot3);
                UnregisterProperptyChanged(u.Slot4);
                UnregisterProperptyChanged(u.Slot5);

                RegisterProperptyChanged(u.Slot1);
                RegisterProperptyChanged(u.Slot2);
                RegisterProperptyChanged(u.Slot3);
                RegisterProperptyChanged(u.Slot4);
                RegisterProperptyChanged(u.Slot5);
            }
        }

        private void RegisterProperptyChanged(OwnedIdol idol)
        {
            if(idol!=null)
            {
                (idol as INotifyPropertyChanged).PropertyChanged += SkillTimeline_PropertyChanged;
            }
        }

        private void UnregisterProperptyChanged(OwnedIdol idol)
        {
            if (idol != null)
            {
                (idol as INotifyPropertyChanged).PropertyChanged -= SkillTimeline_PropertyChanged;
            }
        }
    }
}
