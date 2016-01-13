using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [ImplementPropertyChanged]
    public class Simulator:INotifyPropertyChanged
    {
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

        public bool EnableRoomEffect
        {
            get;
            set;
        }

        public IEnumerable<SongData> SongDataList
        {
            get
            {
                return Song != null ? Song.Data.Values : Enumerable.Empty<SongData>();
            }
        }

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
            get
            {
                return SupportMemberAppeal +
                    CalculateAppeal(Unit.GetValueOrDefault(x=>x.Slot1)) +
                    CalculateAppeal(Unit.GetValueOrDefault(x => x.Slot2)) +
                    CalculateAppeal(Unit.GetValueOrDefault(x => x.Slot3)) +
                    CalculateAppeal(Unit.GetValueOrDefault(x => x.Slot4)) +
                    CalculateAppeal(Unit.GetValueOrDefault(x => x.Slot5)) +
                    CalculateAppeal(Guest);
            }
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

        private List<OwnedIdol> SelectSupportMembers()
        {
            var lst = new List<OwnedIdol>();

            if (Song == null || !EnableSupportMembers||Unit==null)
            {
                return lst;
            }

            IdolCategory preferredCategory = (GrooveBurst == null) ? Song.Type : GrooveType;

            foreach (var item in m_config.OwnedIdols.Where(x => preferredCategory.HasFlag(x.Category)).OrderByDescending(x => CalculateAppeal(x, true)))
            {
                if (lst.Count >= 10)
                {
                    break;
                }
                if(!Unit.OccupiedByUnit(item))
                {
                    lst.Add(item);
                }
            }

            if (lst.Count < 10)
            {
                foreach (var item in m_config.OwnedIdols.OrderByDescending(x => CalculateAppeal(x, true)))
                {
                    if (lst.Count >= 10)
                    {
                        break;
                    }
                    if (!lst.Contains(item)&& !Unit.OccupiedByUnit(item))
                    {
                        lst.Add(item);
                    }
                }
            }

            return lst;
        }

        private int CalculateAppeal(AppealType targetAppeal, IIdol idol, bool isSupportMember,bool encore=false)
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

                if (Unit != null && Unit.Center != null &&
                    Unit.Center.CenterEffect != null && Unit.Center.CenterEffect is CenterEffect.AppealUp)
                {
                    var e = Unit.Center.CenterEffect as CenterEffect.AppealUp;
                    if (e.Targets.HasFlag(idol.Category) && e.TargetAppeal.HasFlag(targetAppeal))
                    {
                        rate += e.Rate;
                    }
                }

                if (Guest != null && Guest.CenterEffect != null && Guest.CenterEffect is CenterEffect.AppealUp)
                {
                    var e = Guest.CenterEffect as CenterEffect.AppealUp;
                    if (e.Targets.HasFlag(idol.Category) && e.TargetAppeal.HasFlag(targetAppeal))
                    {
                        rate += e.Rate;
                    }
                }
            }

            if(GrooveBurst != null)
            {
                if (encore)
                {
                    if(Song != null && Song.Type.HasFlag(idol.Category))
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

        private int CalculateAppeal(IIdol idol, bool isSupportMember=false, bool encore=false)
        {
            return CalculateAppeal(AppealType.Vocal, idol, isSupportMember, encore) +
                CalculateAppeal(AppealType.Dance, idol, isSupportMember, encore) +
                CalculateAppeal(AppealType.Visual, idol, isSupportMember, encore);
        }

        public void Reload()
        {
            SupportMembers = SelectSupportMembers();
            SupportMemberAppeal = SupportMembers.Sum(x => CalculateAppeal(x, true, IsEncore));
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "Song")
            {
                SongData = SongDataList.FirstOrDefault();
            }

            if (propertyName == "EnableSupportMembers"|| propertyName== "GrooveBurst" ||
                propertyName == "GrooveType" || propertyName == "IsEncore" ||
                propertyName == "Guest" || propertyName=="Unit" || 
                propertyName=="Song" || propertyName== "EnableRoomEffect")
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
