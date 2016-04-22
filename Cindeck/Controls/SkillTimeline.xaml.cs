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
        private Dictionary<int, TriggeredSkill[]> m_triggeredSkills=new Dictionary<int, TriggeredSkill[]>();

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
            var skillEffect = idol.Skill == null ? "・" : GetSkillEffect(idol.Skill)+ "・";
            var skillDetail = idol.Skill == null ? "" : GetSkillDetails(idol.Skill, idol.SkillLevel);
            if(SimulationResult==null)
            {
                return name + skillEffect + skillDetail;
            }
            else
            {
                string triggerDetail = "";
                if (idol.Skill != null)
                {
                    int triggered = m_triggeredSkills.ContainsKey(idol.Oid) ? m_triggeredSkills[idol.Oid].Length : 0;
                    int expected = (int)Math.Floor((SimulationResult.Duration - 1.0) / idol.Skill.Interval);
                    triggerDetail = $"{triggered}/{expected}回発動({((double)triggered / expected):P1})";
                }
                return name + skillEffect + skillDetail + "\r\n" + triggerDetail;
            }
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
            if (skill is Skill.Overload)
            {
                return "オーバーロード";
            }
            return null;
        }

        private string GetSkillEffect(ISkill skill)
        {
            if (skill is Skill.ComboBonus)
            {
                var s = skill as Skill.ComboBonus;
                return $"コンボボーナスが{s.Rate:P0}アップ";
            }
            if (skill is Skill.ComboContinuation)
            {
                var s = skill as Skill.ComboContinuation;
                return $"{s.Targets.ToLocalizedString()}でもコンボが継続する";
            }
            if (skill is Skill.DamageGuard)
            {
                var s = skill as Skill.DamageGuard;
                return "ライフが減少しなくなる";
            }
            if (skill is Skill.JudgeEnhancement)
            {
                var s = skill as Skill.JudgeEnhancement;
                return $"{s.Targets.ToLocalizedString()}をPERFECTにする";
            }
            if (skill is Skill.Revival)
            {
                var s = skill as Skill.Revival;
                return $"PERFECTでライフが{s.Amount}回復";
            }
            if (skill is Skill.ScoreBonus)
            {
                var s = skill as Skill.ScoreBonus;
                return $"{s.Targets.ToLocalizedString()}のスコアが{s.Rate:P0}アップ";
            }
            if (skill is Skill.Overload)
            {
                var s = skill as Skill.Overload;
                return $"ライフを{s.ConsumingLife}消費して、スコアが{s.Rate:P0}アップ、{s.ContinuationTargets.ToLocalizedString()}でもCOMBOが継続する";
            }
            return null;
        }

        private string GetSkillDetails(ISkill skill,int lv)
        {
            return $"{skill.Interval}秒ごとに{skill.EstimateProbability(lv):P1}の確率で発動(持続時間{skill.EstimateDuration(lv):F1}秒)";
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
            var playTime = (SimulationResult==null? 120:SimulationResult.Duration)-1;
            var triggerCount = (int)Math.Floor( playTime / interval);
            var totalWidth = c.ActualWidth;
            var skillWidth = (duration / playTime) * totalWidth;
            var intervalWidth = (interval / playTime) * totalWidth;
            var brush = GetBrushBySkill(idol.Skill);
            var disabled = new SolidColorBrush(Colors.LightGray);
            bool notAvail;
            for (int i = 1; i <= triggerCount; i++)
            {
                double left = i * intervalWidth;
                double right = left + skillWidth;

                if (right > c.ActualWidth && skillWidth < (right - c.ActualWidth))
                {
                    break;
                }

                notAvail = SimulationResult != null && !(m_triggeredSkills.ContainsKey(idol.Oid) && m_triggeredSkills[idol.Oid].Any(x => x.Since == i * interval));

                var rect = new Rectangle
                {
                    Fill = notAvail? disabled : brush,
                    Width = right > c.ActualWidth ? skillWidth - (right - c.ActualWidth) : skillWidth,
                    Height = c.ActualHeight,
                    Opacity = 1
                };
                if (rect.Width < 5)
                {
                    left -= 5-rect.Width;
                    rect.Width = 5;
                }
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
            if (skill is Skill.Overload)
            {
                return Resources["OverloadColorBrush"] as Brush;
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
            else if(e.Property==SimulationResultProperty)
            {
                if(e.NewValue!=null)
                {
                    m_triggeredSkills = (e.NewValue as SimulationResult).TriggeredSkills.GroupBy(x => x.Who.Oid).ToDictionary(x=>x.Key,x=>x.ToArray());
                }
                else
                {
                    m_triggeredSkills = new Dictionary<int, TriggeredSkill[]>();
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

        public SimulationResult SimulationResult
        {
            get { return (SimulationResult)GetValue(SimulationResultProperty); }
            set { SetValue(SimulationResultProperty, value); }
        }

        public static readonly DependencyProperty SimulationResultProperty =
                DependencyProperty.Register(nameof(SimulationResult), typeof(SimulationResult), typeof(SkillTimeline), new PropertyMetadata(null));
    }
}
