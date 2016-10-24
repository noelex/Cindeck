using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract]
    public class SimulatorConfig
    {
        [DataMember]
        public string UnitName
        {
            get;
            set;
        }

        [DataMember]
        public bool EnableGuest
        {
            get;
            set;
        }

        [DataMember]
        public int? GuestIid
        {
            get;
            set;
        }

        [DataMember]
        public Potential GuestPotential
        {
            get;
            set;
        }

        [DataMember]
        public bool EnableRoomEffect
        {
            get;
            set;
        }

        [DataMember]
        public bool EnableSupportMembers
        {
            get;
            set;
        }

        [DataMember]
        public string SongTitle
        {
            get;
            set;
        }

        [DataMember]
        public SongDifficulty SongDifficulty
        {
            get;
            set;
        }

        [DataMember]
        public SkillTriggerControl SkillControl
        {
            get;
            set;
        }

        [DataMember]
        public AppealType? GrooveBurst
        {
            get;
            set;
        }

        [DataMember]
        public IdolCategory GrooveType
        {
            get;
            set;
        }

        [DataMember]
        public bool? UtilizeActualPattern
        {
            get;
            set;
        }

        [DataMember]
        public int Runs
        {
            get;
            set;
        }
    }
}
