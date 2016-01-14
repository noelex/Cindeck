using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract]
    public class FilterConfig
    {
        [DataMember]
        public IdolCategory TypeFilter
        {
            get;
            set;
        }

        [DataMember]
        public Rarity? RarityFilter
        {
            get;
            set;
        }

        [DataMember]
        public string NameFilter
        {
            get;
            set;
        }

        [DataMember]
        public string CenterEffectFilter
        {
            get;
            set;
        }

        [DataMember]
        public string SkillFilter
        {
            get;
            set;
        }

        [DataMember]
        public bool FilterOwned
        {
            get;
            set;
        }
    }
}
