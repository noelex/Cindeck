using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public class IdolSource : IIdolSource
    {
        public Task<List<Idol>> GetIdols()
        {
            throw new NotImplementedException();
        }
    }
}
