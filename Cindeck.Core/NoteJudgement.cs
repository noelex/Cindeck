using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [Flags]
    public enum NoteJudgement
    {
        Miss = 0x01,
        Bad = 0x02,
        Nice = 0x04,
        Great = 0x08,
        Perfect = 0x10
    }
}
