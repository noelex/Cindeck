using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.ViewModels
{
    [ImplementPropertyChanged]
    class IdolFilter: INotifyPropertyChanged
    {
        private AppConfig m_config;
        private ICollectionView m_target;

        public IdolFilter(AppConfig config, ICollectionView target, bool allowFilterOwned)
        {
            m_config = config;
            m_target = target;
            target.Filter = FilterIdols;
            AllowFilterOwned = allowFilterOwned;

            IdolTypes = new List<Tuple<IdolCategory, string>>
            {
                Tuple.Create(IdolCategory.All,"全部"),
                Tuple.Create(IdolCategory.Cute,"キュート"),
                Tuple.Create(IdolCategory.Cool,"クール"),
                Tuple.Create(IdolCategory.Passion,"パッション")
            };
            TypeFilter = IdolCategory.All;

            Rarities = typeof(Rarity).GetEnumValues().Cast<Rarity?>().Reverse().Select(x => Tuple.Create(x, x.Value.ToLocalizedString())).ToList();
            Rarities.Insert(0, Tuple.Create(new Rarity?(), "全部"));

            CenterEffects = new List<Tuple<Type, string>>
            {
                Tuple.Create((Type)null,"全部"),
                Tuple.Create(typeof(CenterEffect.AppealUp),"アピールアップ"),
                Tuple.Create(typeof(CenterEffect.SkillTriggerProbabilityUp),"スキル発動率アップ"),
                Tuple.Create(typeof(CenterEffect.LifeUp),"ライフアップ")
            };


            Skills = new List<Tuple<Type, string>>
            {
                Tuple.Create((Type)null,"全部"),
                Tuple.Create(typeof(Skill.ScoreBonus),"スコアボーナス"),
                Tuple.Create(typeof(Skill.ComboBonus),"コンボボーナス"),
                Tuple.Create(typeof(Skill.JudgeEnhancement),"判定強化"),
                Tuple.Create(typeof(Skill.ComboContinuation),"コンボ継続"),
                Tuple.Create(typeof(Skill.Revival),"ライフ回復"),
                Tuple.Create(typeof(Skill.DamageGuard),"ダメージガード")
            };
        }

        public List<Tuple<IdolCategory, string>> IdolTypes
        {
            get;
            private set;
        }

        public List<Tuple<Rarity?, string>> Rarities
        {
            get;
            private set;
        }

        public List<Tuple<Type, string>> CenterEffects
        {
            get;
            private set;
        }

        public List<Tuple<Type, string>> Skills
        {
            get;
            private set;
        }

        public IdolCategory TypeFilter
        {
            get;
            set;
        }

        public Rarity? RarityFilter
        {
            get;
            set;
        }

        public string NameFilter
        {
            get;
            set;
        }

        public Type CenterEffectFilter
        {
            get;
            set;
        }

        public Type SkillFilter
        {
            get;
            set;
        }

        public bool FilterOwned
        {
            get;
            set;
        }

        public bool AllowFilterOwned
        {
            get;
            set;
        }

        public void SetConfig(FilterConfig config)
        {
            if(config==null)
            {
                return;
            }
            TypeFilter = config.TypeFilter;
            RarityFilter = config.RarityFilter;
            NameFilter = config.NameFilter;
            var asm = typeof(CenterEffect).Assembly;
            CenterEffectFilter = string.IsNullOrEmpty(config.CenterEffectFilter) ? null : asm.GetType(config.CenterEffectFilter);
            SkillFilter = string.IsNullOrEmpty(config.SkillFilter) ? null : Type.GetType(config.SkillFilter);
            FilterOwned = config.FilterOwned;
        }

        public FilterConfig GetConfig()
        {
            return new FilterConfig
            {
                TypeFilter = TypeFilter,
                RarityFilter = RarityFilter,
                NameFilter = NameFilter,
                CenterEffectFilter = CenterEffectFilter==null?null: CenterEffectFilter.FullName,
                SkillFilter = SkillFilter==null?null: SkillFilter.FullName,
                FilterOwned = FilterOwned
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool FilterIdols(object obj)
        {
            var idol = obj as IIdol;
            var ok = TypeFilter.HasFlag(idol.Category);

            if (FilterOwned)
            {
                ok &= !m_config.OwnedIdols.Any(x => x.Iid == idol.Iid);
            }

            if (!string.IsNullOrEmpty(NameFilter))
            {
                ok &= idol.Name.ToLower().Contains(NameFilter.ToLower()) ||
                    (idol.Label != null && idol.Label.ToLower().Contains(NameFilter.ToLower()));
            }

            if (RarityFilter != null)
            {
                ok &= idol.Rarity == RarityFilter;
            }

            if (CenterEffectFilter != null)
            {
                ok &= idol.CenterEffect != null && idol.CenterEffect.GetType() == CenterEffectFilter;
            }

            if (SkillFilter != null)
            {
                ok &= idol.Skill != null && idol.Skill.GetType() == SkillFilter;
            }
            return ok;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "FilterOwned" || propertyName == "NameFilter" || propertyName == "RarityFilter" ||
                propertyName == "TypeFilter" || propertyName == "CenterEffectFilter" || propertyName == "SkillFilter")
            {
                m_target.Refresh();
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
