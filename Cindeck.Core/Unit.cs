using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract(IsReference = true)]
    [ImplementPropertyChanged]
    public class Unit
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public OwnedIdol Slot1
        {
            get;
            set;
        }

        [DataMember]
        public OwnedIdol Slot2
        {
            get;
            set;
        }

        /// <summary>
        /// センターです
        /// </summary>
        [DataMember]
        public OwnedIdol Slot3
        {
            get;
            set;
        }

        [DataMember]
        public OwnedIdol Slot4
        {
            get;
            set;
        }

        [DataMember]
        public OwnedIdol Slot5
        {
            get;
            set;
        }

        public OwnedIdol Center
        {
            get
            {
                return Slot3;
            }
        }

        public int Vocal
        {
            get
            {
                return Slot1.GetValueOrDefault(x => x.Vocal) +
                    Slot2.GetValueOrDefault(x => x.Vocal) +
                    Slot3.GetValueOrDefault(x => x.Vocal) +
                    Slot4.GetValueOrDefault(x => x.Vocal) +
                    Slot5.GetValueOrDefault(x => x.Vocal);
            }
        }

        public int Dance
        {
            get
            {
                return Slot1.GetValueOrDefault(x => x.Dance) +
                    Slot2.GetValueOrDefault(x => x.Dance) +
                    Slot3.GetValueOrDefault(x => x.Dance) +
                    Slot4.GetValueOrDefault(x => x.Dance) +
                    Slot5.GetValueOrDefault(x => x.Dance);
            }
        }

        public int Visual
        {
            get
            {
                return Slot1.GetValueOrDefault(x => x.Visual) +
                    Slot2.GetValueOrDefault(x => x.Visual) +
                    Slot3.GetValueOrDefault(x => x.Visual) +
                    Slot4.GetValueOrDefault(x => x.Visual) +
                    Slot5.GetValueOrDefault(x => x.Visual);
            }
        }

        public int Life
        {
            get
            {
                return Slot1.GetValueOrDefault(x => x.Life) +
                    Slot2.GetValueOrDefault(x => x.Life) +
                    Slot3.GetValueOrDefault(x => x.Life) +
                    Slot4.GetValueOrDefault(x => x.Life) +
                    Slot5.GetValueOrDefault(x => x.Life);
            }
        }

        public int TotalAppeal
        {
            get
            {
                return Dance + Vocal + Visual;
            }
        }

        public int TotalAppealWithCenterEffect
        {
            get
            {
                return DanceWithCenterEffect + VocalWithCenterEffect + VisualWithCenterEffect;
            }
        }

        public int VocalWithCenterEffect
        {
            get
            {
                return GetVocal(Slot1) + GetVocal(Slot2) + GetVocal(Slot3) + GetVocal(Slot4) + GetVocal(Slot5);
            }
        }

        public int DanceWithCenterEffect
        {
            get
            {
                return GetDance(Slot1) + GetDance(Slot2) + GetDance(Slot3) + GetDance(Slot4) + GetDance(Slot5);
            }
        }

        public int VisualWithCenterEffect
        {
            get
            {
                return GetVisual(Slot1) + GetVisual(Slot2) + GetVisual(Slot3) + GetVisual(Slot4) + GetVisual(Slot5);
            }
        }

        public int LifeWithCenterEffect
        {
            get
            {
                return GetLife(Slot1) + GetLife(Slot2) + GetLife(Slot3) + GetLife(Slot4) + GetLife(Slot5);
            }
        }

        private int GetLife(OwnedIdol idol)
        {
            if(idol==null)
            {
                return 0;
            }
            if(Center!=null&&Center.CenterEffect!=null&&Center.CenterEffect is CenterEffect.LifeUp&&Center.CenterEffect.Targets.HasFlag(idol.Category))
            {
                return (int)Math.Ceiling(idol.Life + idol.Life * (Center.CenterEffect as CenterEffect.LifeUp).Rate);
            }
            return idol.Life;
        }

        private int GetVocal(OwnedIdol idol)
        {
            if (idol == null)
            {
                return 0;
            }
            if (Center != null && Center.CenterEffect != null && Center.CenterEffect is CenterEffect.AppealUp && Center.CenterEffect.Targets.HasFlag(idol.Category))
            {
                var effect = Center.CenterEffect as CenterEffect.AppealUp;
                if (effect.TargetAppeal.HasFlag(AppealType.Vocal))
                {
                    return (int)Math.Ceiling(idol.Vocal + idol.Vocal * effect.Rate);
                }
            }
            return idol.Vocal;
        }

        private int GetDance(OwnedIdol idol)
        {
            if (idol == null)
            {
                return 0;
            }
            if (Center != null && Center.CenterEffect != null && Center.CenterEffect is CenterEffect.AppealUp && Center.CenterEffect.Targets.HasFlag(idol.Category))
            {
                var effect = Center.CenterEffect as CenterEffect.AppealUp;
                if (effect.TargetAppeal.HasFlag(AppealType.Dance))
                {
                    return (int)Math.Ceiling(idol.Dance + idol.Dance * effect.Rate);
                }
            }
            return idol.Dance;
        }

        private int GetVisual(OwnedIdol idol)
        {
            if (idol == null)
            {
                return 0;
            }
            if (Center != null && Center.CenterEffect != null && Center.CenterEffect is CenterEffect.AppealUp && Center.CenterEffect.Targets.HasFlag(idol.Category))
            {
                var effect = Center.CenterEffect as CenterEffect.AppealUp;
                if (effect.TargetAppeal.HasFlag(AppealType.Visual))
                {
                    return (int)Math.Ceiling(idol.Visual + idol.Visual * effect.Rate);
                }
            }
            return idol.Visual;
        }

        public bool AlreadyInUnit(OwnedIdol idol)
        {
            if (Slot1 != null && Slot1.Iid == idol.Iid) return true;
            if (Slot2 != null && Slot2.Iid == idol.Iid) return true;
            if (Slot3 != null && Slot3.Iid == idol.Iid) return true;
            if (Slot4 != null && Slot4.Iid == idol.Iid) return true;
            if (Slot5 != null && Slot5.Iid == idol.Iid) return true;
            return false;
        }

        public void RemoveIdol(OwnedIdol idol)
        {
            if (Slot1 == idol) Slot1 = null;
            if (Slot2 == idol) Slot2 = null;
            if (Slot3 == idol) Slot3 = null;
            if (Slot4 == idol) Slot4 = null;
            if (Slot5 == idol) Slot5 = null;
        }

        public Unit Clone()
        {
            return new Unit { Name = Name, Slot1 = Slot1, Slot2 = Slot2, Slot3 = Slot3, Slot4 = Slot4, Slot5 = Slot5 };
        }

        public void CopyFrom(Unit unit)
        {
            Name = unit.Name;
            Slot1 = unit.Slot1;
            Slot2 = unit.Slot2;
            Slot3 = unit.Slot3;
            Slot4 = unit.Slot4;
            Slot5 = unit.Slot5;
        }
    }
}
