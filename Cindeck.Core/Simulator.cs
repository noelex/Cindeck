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
        private const double TimeScale = 100;
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
            GuestPotential = new Potential();
            GuestPotential.PropertyChanged += (s, e) => Reload();
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

        public int SupportMemberAppeal
        {
            get;
            private set;
        }

        public int TotalAppeal
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

        private List<OwnedIdol> SelectSupportMembers()
        {
            var lst = new List<OwnedIdol>();

            if (Song == null || !EnableSupportMembers || Unit == null)
            {
                return lst;
            }

            foreach (var item in m_config.OwnedIdols.OrderByDescending(x => CalculateAppeal(x, true)))
            {
                if (lst.Count >= 10)
                {
                    break;
                }
                if (!Unit.OccupiedByUnit(item))
                {
                    lst.Add(item);
                }
            }

            return lst;
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
                life += (int)Math.Ceiling(idol.Life * rate);
            }
            return life;
        }

        private SimulationResult StartSimulation(Random rng, int id)
        {
            var result = new SimulationResult(id);
            if (SongData == null)
                return result;

            int totalScore = 0;
            int notes = 1;
            double scorePerNote = (TotalAppeal * LevelCoefficients[SongData.Level]) / SongData.Notes;
            double notesPerFrame = SongData.Notes / (SongData.Duration * TimeScale);
            int frame = 0;
            int totalFrame = 0;
            int totalLife = Life, maxLife=Life;
            CenterEffect.SkillTriggerProbabilityUp skillRateUp = null;

            if (Unit.Center != null && Unit.Center.CenterEffect != null && Unit.Center.CenterEffect is CenterEffect.SkillTriggerProbabilityUp)
            {
                skillRateUp = Unit.Center.CenterEffect as CenterEffect.SkillTriggerProbabilityUp;
            }

            double comboRate = 1;
            
            List<TriggeredSkill> scoreUp = new List<TriggeredSkill>(),
                comboBonus = new List<TriggeredSkill>(), overload = new List<TriggeredSkill>(),
                damgeGuard=new List<TriggeredSkill>(), revival=new List<TriggeredSkill>();

            while (notes <= SongData.Notes)
            {
                frame++;
                totalFrame++;

                CheckSkillDueTime(totalFrame, scoreUp, comboBonus, overload, damgeGuard, revival);

                if (totalFrame < SongData.Duration * TimeScale)
                {

                    foreach (var slot in Unit.Slots)
                    {
                        if (slot != null && slot.Skill != null)
                        {
                            var sb = slot.Skill as Skill;
                            if (totalFrame % (sb.Interval * TimeScale) == 0)
                            {
                                var propability= sb.EstimateProbability(slot.SkillLevel) * //素の確率
                                                   (1 + (Song.Type.HasFlag(slot.Category) ? 0.3 : 0) +  //属性一致ボーナス
                                                        (skillRateUp != null && skillRateUp.Targets.HasFlag(slot.Category) ? skillRateUp.Rate : 0)); //センター効果

                                if (SkillControl != SkillTriggerControl.NeverTrigger &&
                                    (SkillControl == SkillTriggerControl.AlwaysTrigger || rng.NextDouble() < propability))
                                {
                                    var skill = new TriggeredSkill
                                    {
                                        Who = slot,
                                        Since = totalFrame,
                                        Until = totalFrame + sb.EstimateDuration(slot.SkillLevel) * TimeScale
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

                if (notes <= SongData.Notes && (frame * notesPerFrame >= 1 || totalFrame > SongData.Duration * TimeScale))
                {
                    comboRate = CalculateComboRate(notes, SongData.Notes);

                    totalLife += revival.Select(x => x.Who.Skill).Cast<Skill.Revival>().Select(x => x.Amount).DefaultIfEmpty(0).Max();
                    if (totalLife > maxLife) totalLife = maxLife;

                    var scoreUpRate = 1 + scoreUp.Select(x => x.Rate).Concat(overload.Select(x => x.Rate)).DefaultIfEmpty(0).Max();
                    var comboUpRate = 1 + comboBonus.Select(x => x.Rate).DefaultIfEmpty(0).Max();
                    totalScore += (int)Math.Round(scorePerNote * comboRate * scoreUpRate * comboUpRate);

                    frame = 0;
                    notes++;
                }
            }
            result.Score = totalScore;
            result.TriggeredSkills.ForEach(x =>
            {
                x.Since = Math.Round(x.Since / TimeScale, 1);
                x.Until = Math.Round(x.Until / TimeScale, 1);
            });
            result.Duration = SongData.Duration;
            result.RemainingLife = totalLife;
            result.ScorePerNote = (int)Math.Round((double)totalScore / SongData.Notes);
            ResultsUpToDate = true;
            return result;
        }

        public SimulationResult StartSimulation(Random rng, int id, Queue<Note> pattern=null)
        {
            if(pattern==null)
            {
                return StartSimulation(rng, id);
            }

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

            var currentTime = .0;
            int frame = 0, notes=0;
            while (currentTime<SongData.Duration)
            {
                frame = (int)Math.Round(currentTime * TimeScale);
                CheckSkillDueTime(frame, scoreUp, comboBonus, overload, damgeGuard, revival);
                if (currentTime < SongData.Duration)
                {
                    foreach (var slot in Unit.Slots)
                    {
                        if (slot != null && slot.Skill != null)
                        {
                            var sb = slot.Skill as Skill;
                            if (frame % (sb.Interval * TimeScale) == 0)
                            {
                                var propability = sb.EstimateProbability(slot.SkillLevel) * //素の確率
                                                   (1 + (Song.Type.HasFlag(slot.Category) ? 0.3 : 0) +  //属性一致ボーナス
                                                        (skillRateUp != null && skillRateUp.Targets.HasFlag(slot.Category) ? skillRateUp.Rate : 0)); //センター効果

                                if (SkillControl != SkillTriggerControl.NeverTrigger &&
                                    (SkillControl == SkillTriggerControl.AlwaysTrigger || rng.NextDouble() < propability))
                                {
                                    var skill = new TriggeredSkill
                                    {
                                        Who = slot,
                                        Since = frame,
                                        Until = frame + sb.EstimateDuration(slot.SkillLevel) * TimeScale
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

                if (pattern.Count>0 && pattern.Peek().Time <= currentTime)
                {
                    comboRate = CalculateComboRate(notes, SongData.Notes);

                    totalLife += revival.Select(x => x.Who.Skill).Cast<Skill.Revival>().Select(x => x.Amount).DefaultIfEmpty(0).Max();
                    if (totalLife > maxLife) totalLife = maxLife;

                    var scoreUpRate = 1 + scoreUp.Select(x => x.Rate).Concat(overload.Select(x => x.Rate)).DefaultIfEmpty(0).Max();
                    var comboUpRate = 1 + comboBonus.Select(x => x.Rate).DefaultIfEmpty(0).Max();
                    totalScore += (int)Math.Round(scorePerNote * comboRate * scoreUpRate * comboUpRate);

                    notes++;
                    var note = pattern.Dequeue();

                    if (pattern.Count>0 && note.Time == pattern.Peek().Time)
                    {
                        continue;
                    }
                }

                currentTime += 0.01;
            }

            result.Score = totalScore;
            result.TriggeredSkills.ForEach(x =>
            {
                x.Since = Math.Round(x.Since / TimeScale, 1);
                x.Until = Math.Round(x.Until / TimeScale, 1);
            });
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

        private double GetAppealUpRate(IIdol idol, IIdol center, AppealType targetAppeal)
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
                            conditionFulfilled = Unit.Slots.Any(x => x.Category == IdolCategory.Cool) &&
                                Unit.Slots.Any(x => x.Category == IdolCategory.Cute) &&
                                Unit.Slots.Any(x => x.Category == IdolCategory.Passion);
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

                rate += GetAppealUpRate(idol, Unit?.Center, targetAppeal);
                rate += GetAppealUpRate(idol, Guest, targetAppeal);
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

            switch (targetAppeal)
            {
                case AppealType.Vocal:
                    return (int)Math.Ceiling(idol.Vocal * rate * (isSupportMember ? 0.5 : 1));
                case AppealType.Dance:
                    return (int)Math.Ceiling(idol.Dance * rate * (isSupportMember ? 0.5 : 1));
                case AppealType.Visual:
                    return (int)Math.Ceiling(idol.Visual * rate * (isSupportMember ? 0.5 : 1));
                default:
                    throw new Exception();
            }
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
            SupportMemberAppeal = SupportMembers.Sum(x => CalculateAppeal(x, true, IsEncore));
            
            TotalAppeal = SupportMemberAppeal + Unit.GetValueOrDefault(u => u.Slots.Sum(x => CalculateAppeal(x, false, IsEncore))) + CalculateAppeal(guest, false, IsEncore);
            Life = CalculateLife(Unit, guest);
            ResultsUpToDate = false;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == nameof(Song))
            {
                SongData = SongDataList.FirstOrDefault();
            }

            if (propertyName == nameof(EnableSupportMembers) || propertyName == nameof(GrooveBurst) ||
                propertyName == nameof(GrooveType) || propertyName == nameof(IsEncore) ||
                propertyName == nameof(Guest) || propertyName == nameof(Unit) || propertyName == nameof(SkillControl) ||
                propertyName == nameof(Song) || propertyName == nameof(SongData) || propertyName == nameof(EnableRoomEffect))
            {
                Reload();
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
