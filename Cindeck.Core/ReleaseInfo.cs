using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract]
    public class ReleaseInfo
    {
        [DataMember]
        public string html_url
        {
            get;
            set;
        }

        [DataMember]
        public string tag_name
        {
            get;
            set;
        }
    }
}
