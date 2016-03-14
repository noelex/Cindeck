using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public static class Utils
    {
        private static readonly Dictionary<SkillTriggerProbability, string> m_posNames = new Dictionary<SkillTriggerProbability, string> {
            { SkillTriggerProbability.Low,"低確率"},{ SkillTriggerProbability.Medium,"中確率"} ,{ SkillTriggerProbability.High,"高確率"}  };

        private static readonly Dictionary<SkillDuration, string> m_durationNames = new Dictionary<SkillDuration, string> {
            { SkillDuration.Instantaneous, "一瞬の間"},{ SkillDuration.Short,"わずかな間"} ,{ SkillDuration.Medium,"少しの間"},
            { SkillDuration.Long,"しばらくの間"} ,{ SkillDuration.VeryLong,"かなりの間"}  };

        private static readonly Dictionary<NoteJudgement, string> m_judgeNames = new Dictionary<NoteJudgement, string> {
            { NoteJudgement.Miss,"MISS"},{ NoteJudgement.Bad,"BAD"} ,{ NoteJudgement.Nice,"NICE"} ,
            { NoteJudgement.Great,"GREAT"},{ NoteJudgement.Perfect,"PERFECT"}};

        private static readonly Dictionary<IdolCategory, string> m_idolCats = new Dictionary<IdolCategory, string> {
            { IdolCategory.Cute,"キュート"},{ IdolCategory.Cool,"クール"} ,{ IdolCategory.Passion,"パッション"}, { IdolCategory.All, "全員"} };

        private static readonly Dictionary<IdolCategory, string> m_songCats = new Dictionary<IdolCategory, string> {
            { IdolCategory.Cute,"キュート"},{ IdolCategory.Cool,"クール"} ,{ IdolCategory.Passion,"パッション"}, { IdolCategory.All, "全タイプ"} };

        private static readonly Dictionary<IdolCategory, string> m_idolCatsFull = new Dictionary<IdolCategory, string> {
            { IdolCategory.Cute,"キュートアイドル"},{ IdolCategory.Cool,"クールアイドル"} ,{ IdolCategory.Passion,"パッションアイドル"}, { IdolCategory.All, "全員"} };

        private static readonly Dictionary<AppealType, string> m_appealTypes = new Dictionary<AppealType, string> {
            { AppealType.Vocal,"ボーカル"},{ AppealType.Dance,"ダンス"} ,{ AppealType.Visual,"ビジュアル"}, { AppealType.All, "全"} };

        private static readonly Dictionary<Rarity, string> m_rarities = new Dictionary<Rarity, string> {
            { Rarity.N,"N"},
            { Rarity.NPlus,"N+"},
            { Rarity.R,"R"},
            { Rarity.RPlus,"R+"},
            { Rarity.SR,"SR"},
            { Rarity.SRPlus,"SR+"},
            { Rarity.SSR,"SSR"},
            { Rarity.SSRPlus,"SSR+"}};

        private static readonly Dictionary<SongDifficulty, string> m_songDifficulties = new Dictionary<SongDifficulty, string> {
            { SongDifficulty.Debut,"DEBUT"},
            { SongDifficulty.Regular,"REGULAR"},
            { SongDifficulty.Pro,"PRO"},
            { SongDifficulty.Master,"MASTER" },
            { SongDifficulty.MasterPlus,"MASTER+" }
        };

        private static readonly Dictionary<string, SkillTriggerProbability> m_stringToPos = new Dictionary<string, SkillTriggerProbability> {
            { "低確率",SkillTriggerProbability.Low},
            { "中確率", SkillTriggerProbability.Medium},
            { "高確率", SkillTriggerProbability.High}};

        private static readonly Dictionary<string, SkillDuration> m_stringToDuration = new Dictionary<string, SkillDuration> {
            { "一瞬の間", SkillDuration.Instantaneous},
            { "わずかな間", SkillDuration.Short},
            { "少しの間", SkillDuration.Medium},
            { "しばらくの間", SkillDuration.Long},
            { "かなりの間", SkillDuration.VeryLong}
        };

        private static readonly Dictionary<string, NoteJudgement> m_stringToJudge = new Dictionary<string, NoteJudgement> {
            { "MISS", NoteJudgement.Miss},
            { "BAD", NoteJudgement.Bad} ,
            { "NICE", NoteJudgement.Nice} ,
            { "GREAT", NoteJudgement.Great},
            { "PERFECT", NoteJudgement.Perfect}
        };

        private static readonly Dictionary<string, IdolCategory> m_stringToIdolCat = new Dictionary<string, IdolCategory> {
            { "キュート", IdolCategory.Cute}, { "キュートアイドル", IdolCategory.Cute},
            { "クール", IdolCategory.Cool} ,{ "クールアイドル", IdolCategory.Cool} ,
            { "パッション", IdolCategory.Passion },{ "パッションアイドル", IdolCategory.Passion },
            { "全員", IdolCategory.All} };

        private static readonly Dictionary<string, AppealType> m_stringToAppealType = new Dictionary<string, AppealType> {
            { "ボーカル", AppealType.Vocal},{ "ダンス", AppealType.Dance} ,{ "ビジュアル", AppealType.Visual }, { "全", AppealType.All} };

        private static readonly Dictionary<string, Rarity> m_stringToRarity = new Dictionary<string, Rarity> {
            { "N", Rarity.N},
            { "N+", Rarity.NPlus},
            { "R", Rarity.R},
            { "R+", Rarity.RPlus },
            { "SR", Rarity.SR},
            { "SR+", Rarity.SRPlus},
            { "SSR", Rarity.SSR},
            { "SSR+", Rarity.SSRPlus}};

        private static readonly Dictionary<string, IdolCategory> m_stringToSongType = new Dictionary<string, IdolCategory> {
            { "All", IdolCategory.All},
            { "Co", IdolCategory.Cool},
            { "Cu", IdolCategory.Cute},
            { "Pa", IdolCategory.Passion }};

        private static readonly Dictionary<string, SongDifficulty> m_stringToDifficulty = new Dictionary<string, SongDifficulty> {
            { "DEB", SongDifficulty.Debut},
            { "REG", SongDifficulty.Regular},
            { "PRO", SongDifficulty.Pro},
            { "MAS", SongDifficulty.Master },
            { "MAS+", SongDifficulty.MasterPlus }
        };

        private static Dictionary<SkillTriggerProbability, double> ProbabilityInitialValues = new Dictionary<SkillTriggerProbability, double>
        {
            {SkillTriggerProbability.High,0.50 },
            {SkillTriggerProbability.Medium,0.40 },
            {SkillTriggerProbability.Low,0.30 },
        };

        private static Dictionary<SkillDuration, double> DurationInitialValues = new Dictionary<SkillDuration, double>
        {
            {SkillDuration.VeryLong,6 },
            {SkillDuration.Long,5 },
            {SkillDuration.Medium,4 },
            {SkillDuration.Short,3 },
            {SkillDuration.Instantaneous,2 },
        };

        public static string ToLocalizedString(this SkillTriggerProbability pos)
        {
            return m_posNames[pos];
        }

        public static string ToLocalizedString(this SkillDuration duration)
        {
            return m_durationNames[duration];
        }


        public static string ToLocalizedString(this IdolCategory cat)
        {
            return m_idolCats[cat];
        }

        public static string ToFullLocalizedString(this IdolCategory cat)
        {
            return m_idolCatsFull[cat];
        }

        public static string ToLocalizedString(this AppealType cat)
        {
            return m_appealTypes[cat];
        }

        public static string ToLocalizedString(this Rarity cat)
        {
            return m_rarities[cat];
        }

        public static string ToSongTypeLocalizedString(this IdolCategory t)
        {
            return m_songCats[t];
        }

        public static string ToLocalizedString(this SongDifficulty diff)
        {
            return m_songDifficulties[diff];
        }

        public static string ToLocalizedString(this NoteJudgement judgement, bool descending = true)
        {
            var res = new List<string>();

            var values = judgement.GetType().GetEnumValues().Cast<NoteJudgement>();
            if (descending)
            {
                values = values.OrderByDescending(x => x);
            }
            foreach (var x in values)
            {
                if (judgement.HasFlag(x))
                {
                    res.Add(m_judgeNames[x]);
                }
            }
            return string.Join("/", res.ToArray());
        }

        public static SkillTriggerProbability ToTriggerProbability(this string s)
        {
            return m_stringToPos[s];
        }

        public static SkillDuration ToSkillDuration(this string s)
        {
            return m_stringToDuration[s];
        }

        public static IdolCategory ToIdolCategory(this string s)
        {
            return m_stringToIdolCat[s];
        }

        public static AppealType ToAppealType(this string s)
        {
            return m_stringToAppealType[s];
        }

        public static Rarity ToRarity(this string s)
        {
            return m_stringToRarity[s.Replace('＋', '+')];
        }

        public static IdolCategory ToSongType(this string s)
        {
            return m_stringToSongType[s];
        }

        public static SongDifficulty ToSongDifficulty(this string s)
        {
            return m_stringToDifficulty[s];
        }

        public static NoteJudgement ToJudgement(this string s)
        {
            var res = 0;
            foreach (var x in s.Split('/'))
            {
                res |= (int)m_stringToJudge[x];
            }
            return (NoteJudgement)res;
        }

        public static R GetValueOrDefault<T, R>(this T t, Func<T, R> accessor)
        {
            if (t != null)
            {
                return accessor(t);
            }
            return default(R);
        }

        public static double EstimateProbability(this ISkill skill, int skillLv)
        {
            return ProbabilityInitialValues[skill.TriggerProbability] * Math.Pow(1.05, skillLv - 1);
        }

        public static double EstimateDuration(this ISkill skill, int skillLv)
        {
            return DurationInitialValues[skill.Duration] * Math.Pow(1.05, skillLv - 1);
        }

        public static double CalculateSkillScore(this ISkill skill, int skillLv=10)
        {
            if(!(skill is Skill.ComboBonus)&& !(skill is Skill.ScoreBonus))
            {
                return 0;
            }

            var notes = 700;
            var playTime = 120.0;

            var notesPerSecond = notes / playTime;
            var triggerCount=playTime / skill.Interval;
            var expectedTriggerCount = (int)Math.Floor(skill.EstimateProbability(skillLv) * triggerCount);
            return expectedTriggerCount * skill.EstimateDuration(skillLv) *
                notesPerSecond * (1 + (skill is Skill.ComboBonus ? (skill as Skill.ComboBonus).Rate : (skill as Skill.ScoreBonus).Rate));
        }
    }
}
