using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public interface IIdolSource
    {
        Task<Tuple<List<Idol>, int>> GetIdols();
    }
}
