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

        [DataMember(Name = "debut")]
        public int Debut
        {
            get;
            set;
        }

        [DataMember(Name = "regular")]
        public int Regular
        {
            get;
            set;
        }

        [DataMember(Name = "pro")]
        public int Pro
        {
            get;
            set;
        }

        [DataMember(Name = "master")]
        public int Master
        {
            get;
            set;
        }

        [DataMember(Name = "masterPlus")]
        public int MasterPlus
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
