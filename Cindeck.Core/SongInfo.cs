using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract]
    class SongInfo
    {
        [DataMember(Name = "id")]
        public int Id
        {
            get; set;
        }

        [DataMember(Name = "musicTitle")]
        public string Title
        {
            get; set;
        }

        [DataMember(Name = "type")]
        public int Type
        {
            get;
            set;
        }

        [DataMember(Name = "eventType")]
        public int EventType
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
