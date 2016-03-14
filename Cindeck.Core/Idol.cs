using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(CenterEffect.AppealUp))]
    [KnownType(typeof(CenterEffect.LifeUp))]
    [KnownType(typeof(CenterEffect.SkillTriggerProbabilityUp))]
    [KnownType(typeof(Skill.ComboBonus))]
    [KnownType(typeof(Skill.ComboContinuation))]
    [KnownType(typeof(Skill.JudgeEnhancement))]
    [KnownType(typeof(Skill.DamageGuard))]
    [KnownType(typeof(Skill.Revival))]
    [KnownType(typeof(Skill.ScoreBonus))]
    public class Idol : IIdol
    {
        public int Iid => (Label + Name + Rarity).GetHashCode(); 

        [DataMember]
        public string Label { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        public string LabeledName => string.IsNullOrEmpty(Label) ? Name : $"[{Label}] {Name}";

        [DataMember]
        public Rarity Rarity { get; private set; }

        [DataMember]
        public IdolCategory Category { get; private set; }

        [DataMember]
        public int Life { get; private set; }

        [DataMember]
        public int Dance { get; private set; }

        [DataMember]
        public int Vocal { get; private set; }

        [DataMember]
        public int Visual { get; private set; }

        public int TotalAppeal => Dance + Visual + Vocal;

        [DataMember]
        public ICenterEffect CenterEffect { get; private set; }

        [DataMember]
        public ISkill Skill { get; private set; }

        public double SkillScore => Skill.CalculateSkillScore();

        [DataMember]
        public DateTime ImplementationDate{ get; private set; }

        public Idol(string label,string name,Rarity rarity, IdolCategory cat, int life,int dance,int vocal,int visual,DateTime implemented, ICenterEffect centerEffect,ISkill skill)
        {
            Label = label;
            Name = name;
            Rarity = rarity;
            Category = cat;
            Life = life;
            Dance = dance;
            Vocal = vocal;
            Visual = visual;
            CenterEffect = centerEffect;
            Skill = skill;
            ImplementationDate = implemented;
        }

        public override string ToString()
        {
            return $"{Iid:x8}: [{Rarity.ToLocalizedString()}]{(string.IsNullOrEmpty(Label) ? "" : $"[{Label}]")}{Name}";
        }

        public override bool Equals(object obj)
        {
            return (obj as Idol)?.Iid == Iid;
        }

        public override int GetHashCode()
        {
            return Iid;
        }
    }
}
