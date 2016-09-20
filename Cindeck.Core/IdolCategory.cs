using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public enum IdolCategory
    {
        Cute = 0x01,
        Cool = 0x02,
        Passion = 0x04,
        All = Cute | Cool | Passion
    }
}
