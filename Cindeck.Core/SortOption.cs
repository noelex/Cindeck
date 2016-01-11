using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract]
    public class SortOption
    {
        [DataMember]
        public string Column
        {
            get;
            set;
        }

        [DataMember]
        public ListSortDirection Direction
        {
            get;
            set;
        }
    }
}
