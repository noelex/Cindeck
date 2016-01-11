using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public interface ICenterEffect
    {
        string Name { get; }
        IdolCategory Targets { get; }
        string Description { get; }
    }
}
