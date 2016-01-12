using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public class Simulator
    {
        public Simulator()
        {

        }

        public Unit Unit
        {
            get;
            set;
        }

        public Idol Guest
        {
            get;
            set;
        }

        public List<OwnedIdol> SupportMembers
        {
            get;
            set;
        }

        public bool EnableRoomEffect
        {
            get;
            set;
        }

        public AppealType? GrooveBurst
        {
            get;
            set;
        }
        
    }
}
