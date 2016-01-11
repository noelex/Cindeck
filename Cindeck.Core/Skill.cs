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
    public abstract class Skill : ISkill
    { 
        [DataMember]
        public SkillDuration Duration
        {
            get;
            protected set;
        }

        [DataMember]
        public int Interval
        {
            get;
            protected set;
        }

        [DataMember]
        public string Name
        {
            get;
            protected set;
        }

        [DataMember]
        public SkillTriggerProbability TriggerProbability
        {
            get;
            protected set;
        }

        public abstract string Description
        {
            get;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, Description);
        }

        public static Skill Create(string name, string desc)
        {
            if (string.IsNullOrEmpty(desc))
            {
                return null;
            }

            Skill skill = ScoreBonus.Create(name, desc);
            if (skill != null)
            {
                return skill;
            }

            skill = ComboBonus.Create(name, desc);
            if (skill != null)
            {
                return skill;
            }

            skill = JudgeEnhancement.Create(name, desc);
            if (skill != null)
            {
                return skill;
            }

            skill = ComboContinuation.Create(name, desc);
            if (skill != null)
            {
                return skill;
            }

            skill = Revival.Create(name, desc);
            if (skill != null)
            {
                return skill;
            }
           
            skill = DamageGuard.Create(name, desc);
            if (skill != null)
            {
                return skill;
            }

            throw new FormatException("Unknown skill description: " + desc);
        }

        /// <summary>
        /// 特定のノーツ判定をPERFECTにする
        /// </summary>
        [DataContract]
        public class JudgeEnhancement : Skill
        {
            [DataMember]
            public NoteJudgement Targets
            {
                get;
                private set;
            }

            public override string Description
            {
                get
                {
                    return string.Format("{0}秒ごとに{1}で{2}、{3}をPERFECTにする",
                        Interval, TriggerProbability.ToLocalizedString(), Duration.ToLocalizedString(), Targets.ToLocalizedString());
                }
            }

            public JudgeEnhancement Clone()
            {
                return new JudgeEnhancement
                {
                    Duration = Duration,
                    Interval = Interval,
                    Name = Name,
                    TriggerProbability = TriggerProbability,
                    Targets = Targets
                };
            }

            public static new JudgeEnhancement Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(\d+)秒ご[とど]に(高確率|中確率|低確率)で(一瞬の間|わずかな間|少しの間|しばらくの間|かなりの間)[、。]([A-Z/]+)をPERFECTにする$");
                if (m.Success)
                {
                    return new JudgeEnhancement
                    {
                        Name = name,
                        Interval = int.Parse(m.Groups[1].Value),
                        TriggerProbability = m.Groups[2].Value.ToTriggerProbability(),
                        Duration = m.Groups[3].Value.ToSkillDuration(),
                        Targets = m.Groups[4].Value.ToJudgement()
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// スコアがアップする
        /// </summary>
        [DataContract]
        public class ScoreBonus : Skill
        {
            [DataMember]
            public NoteJudgement Targets
            {
                get;
                private set;
            }

            [DataMember]
            public double Rate
            {
                get;
                private set;
            }

            public override string Description
            {
                get
                {
                    return string.Format("{0}秒ごとに{1}で{2}、{3}のスコアが{4}%アップ",
                            Interval, TriggerProbability.ToLocalizedString(), Duration.ToLocalizedString(), Targets.ToLocalizedString(),(int)(Rate*100));
                }
            }

            public ScoreBonus Clone()
            {
                return new ScoreBonus
                {
                    Duration = Duration,
                    Interval = Interval,
                    Name = Name,
                    TriggerProbability = TriggerProbability,
                    Rate = Rate,
                    Targets=Targets
                };
            }

            public static new ScoreBonus Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(\d+)秒ご[とど]に(高確率|中確率|低確率)で(一瞬の間|わずかな間|少しの間|しばらくの間|かなりの間)[、。]([A-Z/]+)のスコアが(\d+)[%％]アップ$");
                if (m.Success)
                {
                    return new ScoreBonus
                    {
                        Name = name,
                        Interval = int.Parse(m.Groups[1].Value),
                        TriggerProbability = m.Groups[2].Value.ToTriggerProbability(),
                        Duration = m.Groups[3].Value.ToSkillDuration(),
                        Targets = m.Groups[4].Value.ToJudgement(),
                        Rate = int.Parse(m.Groups[5].Value) / 100.0
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// COMBOボーナスがアップする
        /// </summary>
        [DataContract]
        public class ComboBonus : Skill
        {
            [DataMember]
            public double Rate
            {
                get;
                private set;
            }

            public override string Description
            {
                get
                {
                    return string.Format("{0}秒ごとに{1}で{2}、COMBOボーナスが{3}%アップ",
                        Interval, TriggerProbability.ToLocalizedString(), Duration.ToLocalizedString(), (int)(Rate * 100));
                }
            }

            public ComboBonus Clone()
            {
                return new ComboBonus
                {
                    Duration = Duration,
                    Interval = Interval,
                    Name = Name,
                    TriggerProbability = TriggerProbability,
                    Rate=Rate
                };
            }

            public static new ComboBonus Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(\d+)秒ご[とど]に(高確率|中確率|低確率)で(一瞬の間|わずかな間|少しの間|しばらくの間|かなりの間)[、。]COMBOボーナスが(\d+)[%％]アップ$");
                if (m.Success)
                {
                    return new ComboBonus
                    {
                        Name = name,
                        Interval = int.Parse(m.Groups[1].Value),
                        TriggerProbability = m.Groups[2].Value.ToTriggerProbability(),
                        Duration = m.Groups[3].Value.ToSkillDuration(),
                        Rate = int.Parse(m.Groups[4].Value) / 100.0
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// 特定のノーツ判定でもCOMBOが継続する
        /// </summary>
        [DataContract]
        public class ComboContinuation : Skill
        {
            [DataMember]
            public NoteJudgement Targets
            {
                get;
                private set;
            }

            public override string Description
            {
                get
                {
                    return string.Format("{0}秒ごとに{1}で{2}、{3}でもCOMBOが継続する",
                        Interval, TriggerProbability.ToLocalizedString(), Duration.ToLocalizedString(), Targets.ToLocalizedString());
                }
            }

            public ComboContinuation Clone()
            {
                return new ComboContinuation
                {
                    Duration = Duration,
                    Interval = Interval,
                    Name = Name,
                    TriggerProbability = TriggerProbability,
                     Targets=Targets
                };
            }

            public static new ComboContinuation Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(\d+)秒ご[とど]に(高確率|中確率|低確率)で(一瞬の間|わずかな間|少しの間|しばらくの間|かなりの間)[、。]([A-Z/]+)でもCOMBOが継続する$");
                if (m.Success)
                {
                    return new ComboContinuation
                    {
                        Name = name,
                        Interval = int.Parse(m.Groups[1].Value),
                        TriggerProbability = m.Groups[2].Value.ToTriggerProbability(),
                        Duration = m.Groups[3].Value.ToSkillDuration(),
                        Targets = m.Groups[4].Value.ToJudgement()
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// PERFECTでライフが回復
        /// </summary>
        [DataContract]
        public class Revival : Skill
        {
            [DataMember]
            public int Amount
            {
                get;
                private set;
            }

            public override string Description
            {
                get
                {
                    return string.Format("{0}秒ごとに{1}で{2}、PERFECTでライフが{3}回復",
                        Interval, TriggerProbability.ToLocalizedString(), Duration.ToLocalizedString(), Amount);
                }
            }

            public Revival Clone()
            {
                return new Revival
                {
                    Duration = Duration,
                    Interval = Interval,
                    Name = Name,
                    TriggerProbability = TriggerProbability,
                    Amount = Amount
                };
            }

            public static new Revival Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(\d+)秒ご[とど]に(高確率|中確率|低確率)で(一瞬の間|わずかな間|少しの間|しばらくの間|かなりの間)[、。]PERFECTでライフが(\d+)回復$");
                if (m.Success)
                {
                    return new Revival
                    {
                        Name = name,
                        Interval = int.Parse(m.Groups[1].Value),
                        TriggerProbability = m.Groups[2].Value.ToTriggerProbability(),
                        Duration = m.Groups[3].Value.ToSkillDuration(),
                        Amount = int.Parse(m.Groups[4].Value)
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// ライフが減少しなくなる
        /// </summary>
        [DataContract]
        public class DamageGuard : Skill
        {
            public override string Description
            {
                get
                {
                    return string.Format("{0}秒ごとに{1}で{2}、ライフが減少しなくなる",
                        Interval, TriggerProbability.ToLocalizedString(), Duration.ToLocalizedString());
                }
            }

            public DamageGuard Clone()
            {
                return new DamageGuard
                {
                    Duration = Duration,
                    Interval = Interval,
                    Name = Name,
                    TriggerProbability=TriggerProbability
                };
            }

            public static new DamageGuard Create(string name, string desc)
            {
                var m = Regex.Match(desc, @"^(\d+)秒ご[とど]に(高確率|中確率|低確率)で(一瞬の間|わずかな間|少しの間|しばらくの間|かなりの間)[、。]ライフが減少しなくなる$");
                if (m.Success)
                {
                    return new DamageGuard
                    {
                        Name = name,
                        Interval = int.Parse(m.Groups[1].Value),
                        TriggerProbability = m.Groups[2].Value.ToTriggerProbability(),
                        Duration = m.Groups[3].Value.ToSkillDuration()
                    };
                }
                return null;
            }
        }
    }
}