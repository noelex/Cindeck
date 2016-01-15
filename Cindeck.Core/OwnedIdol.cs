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

        public IdolCategory Category => Idol.Category;

        public ICenterEffect CenterEffect => Idol.CenterEffect;

        public int Dance => Idol.Dance;

        public int Iid => Idol.Iid;

        public DateTime ImplementationDate => Idol.ImplementationDate;

        public string Label => Idol.Label;

        [DataMember]
        public int Oid
        {
            get; private set;
        }

        public int Life => Idol.Life;

        public string Name => Idol.Name;

        public Rarity Rarity => Idol.Rarity;

        public ISkill Skill => Idol.Skill;

        [DataMember]
        public int SkillLevel
        {
            get;
            set;
        }

        public int TotalAppeal => Idol.TotalAppeal;

        public int Visual => Idol.Visual;

        public int Vocal => Idol.Vocal;

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
