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

        private static readonly Dictionary<string, AppealUpCondition> m_stringToAppealUpCondition = new Dictionary<string, AppealUpCondition> { { "3タイプ全てのアイドル編成時", AppealUpCondition.UnitContainsAllTypes } };
        private static readonly Dictionary<AppealUpCondition, string> m_appealUpConditionToString = new Dictionary<AppealUpCondition, string> { { AppealUpCondition.UnitContainsAllTypes, "3タイプ全てのアイドル編成時" } };

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

        private static readonly Dictionary<int,Dictionary<Rarity,int>> m_potentialAppealDelta=new Dictionary<int, Dictionary<Rarity, int>>
        {
            { 0,new Dictionary<Rarity, int> { { Rarity.N,0 }, { Rarity.NPlus, 0 }, { Rarity.R, 0 }, { Rarity.RPlus, 0 }, { Rarity.SR, 0 }, { Rarity.SRPlus, 0 }, { Rarity.SSR, 0 }, { Rarity.SSRPlus, 0 } } },
            { 1,new Dictionary<Rarity, int> { { Rarity.N,80 }, { Rarity.NPlus, 80 }, { Rarity.R, 60 }, { Rarity.RPlus, 60 }, { Rarity.SR, 60 }, { Rarity.SRPlus, 60 }, { Rarity.SSR, 40 }, { Rarity.SSRPlus, 40 } } },
            { 2,new Dictionary<Rarity, int> { { Rarity.N,160 }, { Rarity.NPlus, 160 }, { Rarity.R, 120 }, { Rarity.RPlus, 120 }, { Rarity.SR, 120 }, { Rarity.SRPlus, 120 }, { Rarity.SSR, 80 }, { Rarity.SSRPlus, 80 } } },
            { 3,new Dictionary<Rarity, int> { { Rarity.N,250 }, { Rarity.NPlus, 250 }, { Rarity.R, 180 }, { Rarity.RPlus, 180 }, { Rarity.SR, 180 }, { Rarity.SRPlus, 180 }, { Rarity.SSR, 120 }, { Rarity.SSRPlus, 120 } } },
            { 4,new Dictionary<Rarity, int> { { Rarity.N,340 }, { Rarity.NPlus, 340 }, { Rarity.R, 255 }, { Rarity.RPlus, 255 }, { Rarity.SR, 250 }, { Rarity.SRPlus, 250 }, { Rarity.SSR, 170 }, { Rarity.SSRPlus, 170 } } },
            { 5,new Dictionary<Rarity, int> { { Rarity.N,440 }, { Rarity.NPlus, 440 }, { Rarity.R, 330 }, { Rarity.RPlus, 330 }, { Rarity.SR, 320 }, { Rarity.SRPlus, 320 }, { Rarity.SSR, 220 }, { Rarity.SSRPlus, 220 } } },
            { 6,new Dictionary<Rarity, int> { { Rarity.N,540 }, { Rarity.NPlus, 540 }, { Rarity.R, 405 }, { Rarity.RPlus, 405 }, { Rarity.SR, 390 }, { Rarity.SRPlus, 390 }, { Rarity.SSR, 270 }, { Rarity.SSRPlus, 270 } } },
            { 7,new Dictionary<Rarity, int> { { Rarity.N,650 }, { Rarity.NPlus, 650 }, { Rarity.R, 480 }, { Rarity.RPlus, 480 }, { Rarity.SR, 460 }, { Rarity.SRPlus, 460 }, { Rarity.SSR, 320 }, { Rarity.SSRPlus, 320 } } },
            { 8,new Dictionary<Rarity, int> { { Rarity.N,760 }, { Rarity.NPlus, 760 }, { Rarity.R, 570 }, { Rarity.RPlus, 570 }, { Rarity.SR, 540 }, { Rarity.SRPlus, 540 }, { Rarity.SSR, 380 }, { Rarity.SSRPlus, 380 } } },
            { 9,new Dictionary<Rarity, int> { { Rarity.N,880 }, { Rarity.NPlus, 880 }, { Rarity.R, 660 }, { Rarity.RPlus, 660 }, { Rarity.SR, 620 }, { Rarity.SRPlus, 620 }, { Rarity.SSR, 440 }, { Rarity.SSRPlus, 440 } } },
            { 10,new Dictionary<Rarity, int> { { Rarity.N,1000 }, { Rarity.NPlus, 1000 }, { Rarity.R, 750 }, { Rarity.RPlus, 750 }, { Rarity.SR, 700 }, { Rarity.SRPlus, 700 }, { Rarity.SSR, 500 }, { Rarity.SSRPlus, 500 } } },
        };

        private static readonly Dictionary<int, Dictionary<Rarity, int>> m_potentialLifeDelta = new Dictionary<int, Dictionary<Rarity, int>>
        {
            { 0,new Dictionary<Rarity, int> { { Rarity.N,0 }, { Rarity.NPlus, 0 }, { Rarity.R, 0 }, { Rarity.RPlus, 0 }, { Rarity.SR, 0 }, { Rarity.SRPlus, 0 }, { Rarity.SSR, 0 }, { Rarity.SSRPlus, 0 } } },
            { 1,new Dictionary<Rarity, int> { { Rarity.N,1 }, { Rarity.NPlus, 1 }, { Rarity.R, 1 }, { Rarity.RPlus, 1 }, { Rarity.SR, 1 }, { Rarity.SRPlus, 1 }, { Rarity.SSR, 1 }, { Rarity.SSRPlus, 1 } } },
            { 2,new Dictionary<Rarity, int> { { Rarity.N,2 }, { Rarity.NPlus, 2 }, { Rarity.R, 2 }, { Rarity.RPlus, 2 }, { Rarity.SR, 2 }, { Rarity.SRPlus, 2 }, { Rarity.SSR, 2 }, { Rarity.SSRPlus, 2 } } },
            { 3,new Dictionary<Rarity, int> { { Rarity.N,3 }, { Rarity.NPlus, 3 }, { Rarity.R, 3 }, { Rarity.RPlus, 3 }, { Rarity.SR, 4 }, { Rarity.SRPlus, 4 }, { Rarity.SSR, 4 }, { Rarity.SSRPlus, 4 } } },
            { 4,new Dictionary<Rarity, int> { { Rarity.N,4 }, { Rarity.NPlus, 4 }, { Rarity.R, 4 }, { Rarity.RPlus, 4 }, { Rarity.SR, 6 }, { Rarity.SRPlus, 6 }, { Rarity.SSR, 6 }, { Rarity.SSRPlus, 6 } } },
            { 5,new Dictionary<Rarity, int> { { Rarity.N,5 }, { Rarity.NPlus, 5 }, { Rarity.R, 5 }, { Rarity.RPlus, 5 }, { Rarity.SR, 8 }, { Rarity.SRPlus, 8 }, { Rarity.SSR, 8 }, { Rarity.SSRPlus, 8 } } },
            { 6,new Dictionary<Rarity, int> { { Rarity.N,6 }, { Rarity.NPlus, 6 }, { Rarity.R, 6 }, { Rarity.RPlus, 6 }, { Rarity.SR, 10 }, { Rarity.SRPlus, 10 }, { Rarity.SSR, 10 }, { Rarity.SSRPlus, 10 } } },
            { 7,new Dictionary<Rarity, int> { { Rarity.N,7 }, { Rarity.NPlus, 7 }, { Rarity.R, 8 }, { Rarity.RPlus, 8 }, { Rarity.SR, 12 }, { Rarity.SRPlus, 12 }, { Rarity.SSR, 13 }, { Rarity.SSRPlus, 13 } } },
            { 8,new Dictionary<Rarity, int> { { Rarity.N,9 }, { Rarity.NPlus, 9 }, { Rarity.R, 10 }, { Rarity.RPlus, 10 }, { Rarity.SR, 14 }, { Rarity.SRPlus, 14 }, { Rarity.SSR, 16 }, { Rarity.SSRPlus, 16 } } },
            { 9,new Dictionary<Rarity, int> { { Rarity.N,11 }, { Rarity.NPlus, 11 }, { Rarity.R, 12 }, { Rarity.RPlus, 12 }, { Rarity.SR, 17 }, { Rarity.SRPlus, 17 }, { Rarity.SSR, 19 }, { Rarity.SSRPlus, 19 } } },
            { 10,new Dictionary<Rarity, int> { { Rarity.N,13 }, { Rarity.NPlus, 13 }, { Rarity.R, 14 }, { Rarity.RPlus, 14 }, { Rarity.SR, 20 }, { Rarity.SRPlus, 20 }, { Rarity.SSR, 22 }, { Rarity.SSRPlus, 22 } } },
        };

        private static Dictionary<string, Potential> m_potentialDataCache = new Dictionary<string, Potential>();

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

        public static string ToLocalizedString(this AppealUpCondition condition)
        {
            return m_appealUpConditionToString[condition];
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

        public static AppealUpCondition ToAppealUpCondition(this string s)
        {
            return m_stringToAppealUpCondition[s];
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
            var notes = 700;
            var playTime = 120.0;
            var rate = 1.0;

            switch(skill?.GetType().Name)
            {
                case nameof(Skill.ComboBonus):
                    rate += (skill as Skill.ComboBonus).Rate;
                    break;
                case nameof(Skill.ScoreBonus):
                    rate += (skill as Skill.ScoreBonus).Rate;
                    break;
                case nameof(Skill.Overload):
                    rate += (skill as Skill.Overload).Rate;
                    break;
                default:
                    return 0;
            }

            var notesPerSecond = notes / playTime;
            var triggerCount=playTime / skill.Interval;
            var expectedTriggerCount = (int)Math.Floor(skill.EstimateProbability(skillLv) * triggerCount);
            return expectedTriggerCount * skill.EstimateDuration(skillLv) * notesPerSecond * rate;
        }

        private static Potential GetPotential(string idolName)
        {
            if(m_potentialDataCache.ContainsKey(idolName))
            {
                return m_potentialDataCache[idolName];
            }
            return m_potentialDataCache[idolName] = AppConfig.Current.PotentialData.First(x => x.Name == idolName);
        }

        public static int GetVocalWithPotential(this IIdol idol, Potential potential = null)
        {
            return idol.Vocal + m_potentialAppealDelta[(potential ?? GetPotential(idol.Name)).Vocal][idol.Rarity];
        }

        public static int GetDanceWithPotential(this IIdol idol, Potential potential = null)
        {
            return idol.Dance + m_potentialAppealDelta[(potential ?? GetPotential(idol.Name)).Dance][idol.Rarity];
        }

        public static int GetVisualWithPotential(this IIdol idol, Potential potential = null)
        {
            return idol.Visual + m_potentialAppealDelta[(potential ?? GetPotential(idol.Name)).Visual][idol.Rarity];
        }

        public static int GetLifeWithPotential(this IIdol idol, Potential potential = null)
        {
            return idol.Life + m_potentialLifeDelta[(potential?? GetPotential(idol.Name)).Life][idol.Rarity];
        }
    }
}
