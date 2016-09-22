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

        public double Time
        {
            get;
            set;
        }

        [DataMember(Name = "sec")]
        private string TimeString
        {
            get { return Time.ToString(); }
            set { Time = string.IsNullOrWhiteSpace(value) ? 0 : double.Parse(value); }
        }

        [DataMember(Name = "type")]
        public int Type { get; set; }
    }
}
