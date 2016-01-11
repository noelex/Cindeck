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
    public class OwnedIdol:IIdol
    {
        [DataMember]
        private Idol Idol
        {
            get;
            set;
        }

        public OwnedIdol(int lid, Idol idol, int skillLevel=10)
        {
            Oid = lid;
            Idol = idol;
            SkillLevel = skillLevel;
        }

        public IdolCategory Category
        {
            get
            {
                return Idol.Category;
            }
        }

        public ICenterEffect CenterEffect
        {
            get
            {
                return Idol.CenterEffect;
            }
        }

        public int Dance
        {
            get
            {
                return Idol.Dance;
            }
        }

        public int Iid
        {
            get
            {
                return Idol.Iid;
            }
        }

        public DateTime ImplementationDate
        {
            get
            {
                return Idol.ImplementationDate;
            }
        }

        public string Label
        {
            get
            {
                return Idol.Label;
            }
        }

        [DataMember]
        public int Oid
        {
            get;
            private set;
        }

        public int Life
        {
            get
            {
                return Idol.Life;
            }
        }

        public string Name
        {
            get
            {
                return Idol.Name;
            }
        }

        public Rarity Rarity
        {
            get
            {
                return Idol.Rarity;
            }
        }

        public ISkill Skill
        {
            get
            {
                return Idol.Skill;
            }
        }

        [DataMember]
        public int SkillLevel
        {
            get;
            set;
        }

        public int TotalAppeal
        {
            get
            {
                return Idol.TotalAppeal;
            }
        }

        public int Visual
        {
            get
            {
                return Idol.Visual;
            }
        }

        public int Vocal
        {
            get
            {
                return Idol.Vocal;
            }
        }

        public void UpdateReference(Idol idol)
        {
            if(idol.Iid!=Idol.Iid)
            {
                throw new Exception("Cannot update reference to an idol with difference IID.");
            }
            Idol = idol;
        }
    }
}
