using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public class TriggeredSkill
    {
        public OwnedIdol Who
        {
            get;
            set;
        }

        public double Rate
        {
            get
            {
                if (Who.Skill is Skill.ScoreBonus) return (Who.Skill as Skill.ScoreBonus).Rate;
                if (Who.Skill is Skill.ComboBonus) return (Who.Skill as Skill.ComboBonus).Rate;
                if (Who.Skill is Skill.Overload) return (Who.Skill as Skill.Overload).Rate;
                throw new NotSupportedException();
            }
        }

        public double Until
        {
            get;
            set;
        }

        public double Since
        {
            get;
            set;
        }
    }

    public class SimulationResult
    {
        public SimulationResult(int id)
        {
            Id = id;
            TriggeredSkills = new List<TriggeredSkill>();
        }

        public int Id
        {
            get;
        }

        public string Name => $"{Id}回目";

        public int Score
        {
            get;
            set;
        }
        public int ScorePerNote
        {
            get; set;
        }

        public List<TriggeredSkill> TriggeredSkills
        {
            get;
        }

        public int Duration
        {
            get;
            set;
        }

        public int RemainingLife
        {
            get;
            set;
        }
    }

    public enum SkillTriggerControl
    {
        Auto,
        AlwaysTrigger,
        NeverTrigger
    }

    [ImplementPropertyChanged]
    public class Simulator : INotifyPropertyChanged
    {
        private const int TimeScale = 100;
        private static readonly Dictionary<int, double> LevelCoefficients = new Dictionary<int, double>
    {
            // DEBUT
            { 5, 1 }, { 6, 1.025 }, { 7, 1.05 }, { 8, 1.075 }, { 9, 1.1 },
            // REGULAR
            { 10, 1.2 }, { 11, 1.225 }, { 12, 1.25 }, { 13, 1.275 }, { 14, 1.3 },
            // PRO
            { 15, 1.4 }, { 16, 1.425 }, { 17, 1.45 }, { 18, 1.475 }, { 19, 1.5 },
            // MASTER
            { 20, 1.6 }, { 21, 1.65 }, { 22, 1.7 }, { 23, 1.75 },{ 24, 1.8 }, {25,1.85 }, { 26, 1.9 }, { 27, 1.95 }, { 28, 2 },
            // MASTER+
            { 29, 2.1 }, { 30, 2.2 }
        };

        private AppConfig m_config;

        public event PropertyChangedEventHandler PropertyChanged;

        public Simulator(AppConfig config)
        {
            m_config = config;
            GrooveType = IdolCategory.Cute;
        }

        public Idol Guest
        {
            get;
            set;
        }

        public Potential GuestPotential
        {
            get;
            set;
        }

        public bool EnableRoomEffect
        {
            get;
            set;
        }

        public IEnumerable<SongData> SongDataList =>
            Song != null ? Song.Data.Values : Enumerable.Empty<SongData>();

        public Song Song
        {
            get;
            set;
        }

        public SongData SongData
        {
            get;
            set;
        }

        public bool EnableSupportMembers
        {
            get;
            set;
        }

        public List<OwnedIdol> SupportMembers
        {
            get;
            private set;
        }

        public int SupportMemberAppeal => SupportMemberVocalAppeal + SupportMemberDanceAppeal + SupportMemberVisualAppeal;

        public int SupportMemberVocalAppeal
        {
            get;
            private set;
        }

        public int SupportMemberDanceAppeal
        {
            get;
            private set;
        }

        public int SupportMemberVisualAppeal
        {
            get;
            private set;
        }

        public int TotalAppeal => VocalAppeal + DanceAppeal + VisualAppeal;

        public int VocalAppeal
        {
            get;
            private set;
        }

        public int DanceAppeal
        {
            get;
            private set;
        }

        public int VisualAppeal
        {
            get;
            private set;
        }

        public int Life
        {
            get;
            private set;
        }


        public Unit Unit
        {
            get;
            set;
        }

        public AppealType? GrooveBurst
        {
            get;
            set;
        }

        public IdolCategory GrooveType
        {
            get;
            set;
        }

        public bool IsEncore
        {
            get;
            set;
        }

        public bool ResultsUpToDate
        {
            get;
            set;
        }

        public SkillTriggerControl SkillControl
        {
            get;
            set;
        }

        public Dictionary<string, double> ExpectedTriggerRatio
        {
            get;
            private set;
        }

        private List<OwnedIdol> SelectSupportMembers()
        {
            if (Song == null || !EnableSupportMembers || Unit == null)
            {
                return new List<OwnedIdol>();
            }

            return m_config.OwnedIdols.Where(x => !Unit.OccupiedByUnit(x))
                .OrderByDescending(x => CalculateAppeal(x, true)).Take(10).ToList();
        }

        private void CheckSkillDueTime(double frame, params List<TriggeredSkill>[] skillLists)
        {
            foreach(var skillList in skillLists)
            {
                foreach (var s in skillList.ToArray())
                {
                    if (frame > s.Until)
                    {
                        skillList.Remove(s);
                    }
                }
            }
        }

        private int CalculateLife(Unit unit, Idol guest)
        {
            if(unit==null)
            {
                return 0;
            }

            var life = 0;
            var centerEffect = unit.Center?.CenterEffect is CenterEffect.LifeUp ? (CenterEffect.LifeUp)unit.Center.CenterEffect : null;
            var guestCenterEffect = guest?.CenterEffect is CenterEffect.LifeUp ? (CenterEffect.LifeUp)guest.CenterEffect : null;

            foreach (var idol in unit.Slots.Cast<IIdol>().Concat(Enumerable.Repeat(guest,1)))
            {
                if (idol == null) continue;

                var rate = 1.0;
                if (centerEffect!=null && centerEffect.Targets.HasFlag(idol.Category) == true)
                {
                    rate += centerEffect.Rate;
                }
                if (guestCenterEffect != null && guestCenterEffect.Targets.HasFlag(idol.Category) == true)
                {
                    rate += guestCenterEffect.Rate;
                }
                life += (int)Math.Ceiling(Math.Round(idol.Life * rate, 3));
            }
            return life;
        }

        public SimulationResult StartSimulation(Random rng, int id, Queue<Note> pattern=null)
        {
            var result = new SimulationResult(id);
            if (SongData == null)
                return result;

            int totalScore = 0;
            double scorePerNote = (TotalAppeal * LevelCoefficients[SongData.Level]) / SongData.Notes;
            int totalLife = Life, maxLife = Life;
            CenterEffect.SkillTriggerProbabilityUp skillRateUp = null;

            if (Unit.Center != null && Unit.Center.CenterEffect != null && Unit.Center.CenterEffect is CenterEffect.SkillTriggerProbabilityUp)
            {
                skillRateUp = Unit.Center.CenterEffect as CenterEffect.SkillTriggerProbabilityUp;
            }

            double comboRate = 1;

            List<TriggeredSkill> scoreUp = new List<TriggeredSkill>(),
                comboBonus = new List<TriggeredSkill>(), overload = new List<TriggeredSkill>(),
                damgeGuard = new List<TriggeredSkill>(), revival = new List<TriggeredSkill>();

            if (pattern == null)
            {
                var interval = (double)SongData.Duration / SongData.Notes;
                pattern = new Queue<Note>(Enumerable.Range(1, SongData.Notes).Select(x => new Note { Id = x, Time = x * interval }));
            }

            double currentTime = .01;
            int notes=0;
            bool sync = false;
            while (currentTime <= SongData.Duration)
            {
                CheckSkillDueTime(currentTime, scoreUp, comboBonus, overload, damgeGuard, revival);
                if (currentTime < SongData.Duration && !sync)
                {
                    var f = (int)Math.Round(currentTime * TimeScale);
                    foreach (var slot in Unit.Slots)
                    {
                        if (slot != null && slot.Skill != null)
                        {
                            var sb = slot.Skill as Skill;
                            if (f % (sb.Interval*TimeScale) == 0)
                            {
                                //var propability = sb.EstimateProbability(slot.SkillLevel) * //素の確率
                                //                   (1 + (Song.Type.HasFlag(slot.Category) ? 0.3 : 0) +  //属性一致ボーナス
                                //                        (skillRateUp != null && skillRateUp.Targets.HasFlag(slot.Category) ? skillRateUp.Rate : 0)); //センター効果
                                var propability = GetSkillTriggerProbability(slot, Unit.Center, Guest, Song);
                                if (SkillControl != SkillTriggerControl.NeverTrigger &&
                                    (SkillControl == SkillTriggerControl.AlwaysTrigger || rng.NextDouble() < propability))
                                {
                                    var skill = new TriggeredSkill
                                    {
                                        Who = slot,
                                        Since = currentTime,
                                        Until = currentTime + sb.EstimateDuration(slot.SkillLevel)
                                    };

                                    switch (sb.GetType().Name)
                                    {
                                        case nameof(Skill.DamageGuard):
                                            damgeGuard.Add(skill);
                                            break;
                                        case nameof(Skill.Revival):
                                            revival.Add(skill);
                                            break;
                                        case nameof(Skill.ScoreBonus):
                                            scoreUp.Add(skill);
                                            break;
                                        case nameof(Skill.ComboBonus):
                                            comboBonus.Add(skill);
                                            break;
                                        case nameof(Skill.Overload):
                                            var o = sb as Skill.Overload;
                                            if (totalLife - o.ConsumingLife > 0)
                                            {
                                                if (!damgeGuard.Any())
                                                {
                                                    totalLife -= o.ConsumingLife;
                                                }
                                                overload.Add(skill);
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    result.TriggeredSkills.Add(skill);
                                }
                            }
                        }
                    }
                }

                if (pattern.Count > 0 && pattern.Peek().Time <= currentTime)
                {
                    comboRate = CalculateComboRate(notes, SongData.Notes);

                    totalLife += revival.Select(x => x.Who.Skill).Cast<Skill.Revival>().Select(x => x.Amount).DefaultIfEmpty(0).Max();
                    if (totalLife > maxLife) totalLife = maxLife;

                    var scoreUpRate = 1 + scoreUp.Select(x => x.Rate).Concat(overload.Select(x => x.Rate)).DefaultIfEmpty(0).Max();
                    var comboUpRate = 1 + comboBonus.Select(x => x.Rate).DefaultIfEmpty(0).Max();
                    totalScore += (int)Math.Round(scorePerNote * comboRate * scoreUpRate * comboUpRate);

                    notes++;
                    var note = pattern.Dequeue();

                    if (pattern.Count > 0 && note.Time == pattern.Peek().Time)
                    {
                        sync = true;
                        continue;
                    }
                    sync = false;
                }

                currentTime += 0.01;
            }

            result.Score = totalScore;
            result.Duration = SongData.Duration;
            result.RemainingLife = totalLife;
            result.ScorePerNote = (int)Math.Round((double)totalScore / SongData.Notes);
            ResultsUpToDate = true;
            return result;
        }

        private double CalculateComboRate(int comboNotes, int totalNotes)
        {
            var progress = (double)comboNotes / totalNotes;
            if (progress >= 0.9)
            {
                return 2;
            }
            else if (progress >= 0.8)
            {
                return 1.7;
            }
            else if (progress >= 0.7)
            {
                return 1.5;
            }
            else if (progress >= 0.5)
            {
                return 1.4;
            }
            else if (progress >= 0.25)
            {
                return 1.3;
            }
            else if (progress >= 0.1)
            {
                return 1.2;
            }
            else if (progress >= 0.05)
            {
                return 1.1;
            }
            else
            {
                return 1;
            }
        }

        private double GetAppealUpRate(IIdol idol, IIdol center, Idol guest, AppealType targetAppeal)
        {
            var effect = center?.CenterEffect;

            if (effect != null && Unit != null)
            {
                if (effect is CenterEffect.AppealUp)
                {
                    var e = effect as CenterEffect.AppealUp;
                    if (e.Targets.HasFlag(idol.Category) == true && e.TargetAppeal.HasFlag(targetAppeal) == true)
                    {
                        return e.Rate;
                    }
                }
                else if (effect is CenterEffect.ConditionalAppealUp)
                {
                    var e = effect as CenterEffect.ConditionalAppealUp;
                    var conditionFulfilled = false;
                    switch (e.Condition)
                    {
                        case AppealUpCondition.UnitContainsAllTypes:
                            conditionFulfilled = (Unit.Slots.Any(x => x.Category == IdolCategory.Cool) || (Guest?.Category == IdolCategory.Cool)) &&
                                (Unit.Slots.Any(x => x.Category == IdolCategory.Cute) || (Guest?.Category == IdolCategory.Cute)) &&
                                (Unit.Slots.Any(x => x.Category == IdolCategory.Passion) || (Guest?.Category == IdolCategory.Passion));
                            break;
                        default:
                            break;
                    }

                    if (conditionFulfilled && e.Targets.HasFlag(idol.Category) == true && e.TargetAppeal.HasFlag(targetAppeal) == true)
                    {
                        return e.Rate;
                    }
                }
            }

            return 0;
        }

        private double GetSkillTriggerProbability(OwnedIdol idol, IIdol center, IIdol guestCenter, Song song)
        {
            if (idol == null || idol.Skill == null)
            {
                return 0;
            }
            var rate = idol.Skill.EstimateProbability(idol.SkillLevel);
            if (center != null && center.CenterEffect is CenterEffect.SkillTriggerProbabilityUp)
            {
                var e = center.CenterEffect as CenterEffect.SkillTriggerProbabilityUp;
                rate *= 1+(e.Targets.HasFlag(idol.Category) ? e.Rate : 0);
            }
            if (guestCenter != null && guestCenter.CenterEffect is CenterEffect.SkillTriggerProbabilityUp)
            {
                var e = guestCenter.CenterEffect as CenterEffect.SkillTriggerProbabilityUp;
                rate *= 1+(e.Targets.HasFlag(idol.Category) ? e.Rate : 0);
            }
            if (song != null && song.Type.HasFlag(idol.Category))
            {
                rate *= 1.3;
            }
            return Math.Min(rate, 1.0);
        }

        private int CalculateAppeal(AppealType targetAppeal, IIdol idol, bool isSupportMember, bool encore = false)
        {
            if (idol == null)
            {
                return 0;
            }
            var rate = 1.0;

            if (!isSupportMember)
            {
                if (EnableRoomEffect)
                {
                    rate += 0.1;
                }

                rate += GetAppealUpRate(idol, Unit?.Center, Guest, targetAppeal);
                rate += GetAppealUpRate(idol, Guest, Guest, targetAppeal);
            }

            if (GrooveBurst != null)
            {
                if (encore)
                {
                    if (Song != null && Song.Type.HasFlag(idol.Category))
                    {
                        rate += 0.3;
                    }
                }
                else if (GrooveType.HasFlag(idol.Category))
                {
                    rate += 0.3;
                }

                if (GrooveBurst.Value.HasFlag(targetAppeal))
                {
                    rate += 1.5;
                }
            }
            else if (Song != null && Song.Type.HasFlag(idol.Category))
            {
                rate += 0.3;
            }

            return (int)Math.Ceiling(Math.Round((int)idol.GetType().GetProperty(targetAppeal.ToString()).GetValue(idol) * rate * (isSupportMember ? 0.5 : 1), 3));
        }

        private int CalculateAppeal(IIdol idol, bool isSupportMember = false, bool encore = false)
        {
            return CalculateAppeal(AppealType.Vocal, idol, isSupportMember, encore) +
                CalculateAppeal(AppealType.Dance, idol, isSupportMember, encore) +
                CalculateAppeal(AppealType.Visual, idol, isSupportMember, encore);
        }

        private Idol CreateGuestWithPotential(IIdol guest)
        {
            return guest == null ? null : new Idol(guest.Label, guest.Name, guest.Rarity, guest.Category, guest.GetLifeWithPotential(GuestPotential), guest.GetDanceWithPotential(GuestPotential),
                guest.GetVocalWithPotential(GuestPotential), guest.GetVisualWithPotential(GuestPotential), guest.ImplementationDate, guest.CenterEffect, guest.Skill);
        }

        public void Reload()
        {
            var guest = CreateGuestWithPotential(Guest);

            SupportMembers = SelectSupportMembers();
            SupportMemberVocalAppeal = SupportMembers.Sum(x => CalculateAppeal(AppealType.Vocal, x, true, IsEncore));
            SupportMemberDanceAppeal = SupportMembers.Sum(x => CalculateAppeal(AppealType.Dance, x, true, IsEncore));
            SupportMemberVisualAppeal = SupportMembers.Sum(x => CalculateAppeal(AppealType.Visual, x, true, IsEncore));

            VocalAppeal = SupportMemberVocalAppeal + CalculateAppeal(AppealType.Vocal, guest, false, IsEncore) +
                Unit.GetValueOrDefault(u => u.Slots.Sum(x => CalculateAppeal(AppealType.Vocal, x, false, IsEncore)));
            DanceAppeal = SupportMemberDanceAppeal + CalculateAppeal(AppealType.Dance, guest, false, IsEncore) +
                Unit.GetValueOrDefault(u => u.Slots.Sum(x => CalculateAppeal(AppealType.Dance, x, false, IsEncore)));
            VisualAppeal = SupportMemberVisualAppeal + CalculateAppeal(AppealType.Visual, guest, false, IsEncore) +
                Unit.GetValueOrDefault(u => u.Slots.Sum(x => CalculateAppeal(AppealType.Visual, x, false, IsEncore)));

            if (Unit != null)
            {
                int i = 1;
                ExpectedTriggerRatio = Unit.Slots.ToDictionary(
                    x => $"スロット{i++}",
                    x =>
                    {
                        if (SkillControl == SkillTriggerControl.NeverTrigger || x == null || x.Skill == null)
                        {
                            return .0;
                        }
                        else if (SkillControl == SkillTriggerControl.AlwaysTrigger)
                        {
                            return 1.0;
                        }
                        else
                        {
                            return GetSkillTriggerProbability(x, Unit.Center, Guest, Song);
                            //return x.Skill.EstimateProbability(x.SkillLevel) * (1 + skillUpRate.GetValueOrDefault(0) + (songType != null && songType.Value.HasFlag(x.Category) ? 0.3 : 0));
                        }
                    });
            }

            Life = CalculateLife(Unit, guest);
            ResultsUpToDate = false;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == nameof(Song))
            {
                SongData = SongDataList.Where(x=>x.Difficulty!=SongDifficulty.MasterPlus).OrderByDescending(x=>x.Difficulty).FirstOrDefault();
            }

            if (propertyName == nameof(EnableSupportMembers) || propertyName == nameof(GrooveBurst) ||
                propertyName == nameof(GrooveType) || propertyName == nameof(IsEncore) ||
                propertyName == nameof(Guest) || propertyName == nameof(Unit) || propertyName == nameof(SkillControl) ||
                propertyName == nameof(Song) || propertyName == nameof(SongData) || propertyName == nameof(EnableRoomEffect))
            {
                Reload();
            }

            if (propertyName == nameof(GuestPotential) && after != null)
            {
                GuestPotential.PropertyChanged += (s, e) => Reload();
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
