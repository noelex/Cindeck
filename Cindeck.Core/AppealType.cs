using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public enum AppealType
    {
        Vocal = 0x01,
        Dance = 0x02,
        Visual = 0x04,
        All = Vocal | Dance | Visual
    }
}
