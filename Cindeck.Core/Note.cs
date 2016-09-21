using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract]
    public class Note
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "sec")]
        public double Time { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }
    }
}
