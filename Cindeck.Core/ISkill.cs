using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public interface ISkill
    {
        string Name { get; }
        SkillDuration Duration { get; }
        SkillTriggerProbability TriggerProbability { get; }
        int Interval { get; }
        string Description { get; }
    }
}
