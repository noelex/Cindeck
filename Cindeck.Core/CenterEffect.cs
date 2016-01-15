using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract]
    public abstract class CenterEffect : ICenterEffect
    {
        [DataMember]
        public string Name
        {
            get;
            private set;
        }

        [DataMember]
        public IdolCategory Targets
        {
            get;
            private set;
        }
        
        public abstract string Description
        {
            get;
        }

        public override string ToString()
        {
            return $"{Name}: {Description}";
        }

        public static CenterEffect Create(string name, string desc)
        {
            if(string.IsNullOrEmpty(desc))
            {
                return null;
            }

            CenterEffect effect = AppealUp.Create(name, desc);
            if (effect != null)
            {
                return effect;
            }

            effect = SkillTriggerProbabilityUp.Create(name, desc);
            if (effect != null)
            {
                return effect;
            }

            effect = LifeUp.Create(name, desc);
            if (effect != null)
            {
                return effect;
            }

            throw new FormatException("Unknown effect description: " + desc);
        }

        [DataContract]
        public class AppealUp : CenterEffect
        {
            [DataMember]
            public double Rate
            {
                get;
                private set;
            }

            [DataMember]
            public AppealType TargetAppeal
            {
                get;
                private set;
            }

            public override string Description=>
                    $"{Targets.ToFullLocalizedString()}の{TargetAppeal.ToLocalizedString()}アピール値{Rate:P0}アップ";

            public static new AppealUp Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(キュートアイドル|クールアイドル|パッションアイドル|全員)の(ダンス|ボーカル|ビジュアル|全)(アピール)?値[が]?(\d+)[%％]アップ$");
                if (m.Success)
                {
                    return new AppealUp
                    {
                        Name = name,
                        Targets = m.Groups[1].Value.ToIdolCategory(),
                        TargetAppeal = m.Groups[2].Value.ToAppealType(),
                        Rate = int.Parse(m.Groups[4].Value) / 100.0,
                    };
                }
                return null;
            }

            public AppealUp Clone()
            {
                return new AppealUp
                {
                    Name = Name,
                    Rate = Rate,
                    Targets = Targets,
                    TargetAppeal=TargetAppeal
                };
            }
        }

        [DataContract]
        public class LifeUp:CenterEffect
        {
            [DataMember]
            public double Rate
            {
                get;
                private set;
            }

            public override string Description=>
                    $"{Targets.ToFullLocalizedString()}のライフ{Rate:P0}アップ";

            public static new LifeUp Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(キュートアイドル|クールアイドル|パッションアイドル|全員)のライフ(\d+)[%％]アップ$");
                if (m.Success)
                {
                    return new LifeUp
                    {
                        Name = name,
                        Targets = m.Groups[1].Value.ToIdolCategory(),
                        Rate = int.Parse(m.Groups[2].Value) / 100.0,
                    };
                }
                return null;
            }

            public LifeUp Clone()
            {
                return new LifeUp
                {
                    Name = Name,
                    Rate = Rate,
                    Targets = Targets
                };
            }
        }

        [DataContract]
        public class SkillTriggerProbabilityUp : CenterEffect
        {
            [DataMember]
            public double Rate
            {
                get;
                private set;
            }

            public override string Description =>
                    $"{Targets.ToFullLocalizedString()}の特技発動確率{Rate:P0}アップ";

            public SkillTriggerProbabilityUp Clone()
            {
                return new SkillTriggerProbabilityUp
                {
                    Name = Name,
                    Rate = Rate,
                    Targets = Targets
                };
            }

            public static new SkillTriggerProbabilityUp Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(キュートアイドル|クールアイドル|パッションアイドル|全員)の特技発動確率(\d+)[%％]アップ$");
                if (m.Success)
                {
                    return new SkillTriggerProbabilityUp
                    {
                        Name = name,
                        Targets = m.Groups[1].Value.ToIdolCategory(),
                        Rate = int.Parse(m.Groups[2].Value)/100.0,
                    };
                }
                return null;
            }
        }
    }
}
