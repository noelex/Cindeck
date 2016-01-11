using System;

namespace Cindeck.Core
{
    public interface IIdol
    {
        IdolCategory Category { get; }
        ICenterEffect CenterEffect { get; }
        int Dance { get; }
        int Iid { get; }
        DateTime ImplementationDate { get; }
        string Label { get; }
        int Life { get; }
        string Name { get; }
        Rarity Rarity { get; }
        ISkill Skill { get; }
        int TotalAppeal { get; }
        int Visual { get; }
        int Vocal { get; }
    }
}