using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public enum AppealType
    {
        None=0,
        Vocal = 0x01,
        Dance = 0x02,
        Visual = 0x04,
        All = Vocal | Dance | Visual
    }
}
