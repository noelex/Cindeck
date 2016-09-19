using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cindeck.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core.Tests
{
    [TestClass()]
    public class SkillTests
    {
        [TestMethod()]
        public void TestCreateScoreBonus()
        {
            var name = "きらりん☆キャンディ";
            var desc = "9秒ごとに高確率で少しの間、PERFECT/GREATのスコアが17%アップ";
            var skill = Skill.Create(name, desc);
            Assert.IsNotNull(skill);
            Assert.IsInstanceOfType(skill, typeof(Skill.ScoreBonus));

            var sb = (Skill.ScoreBonus)skill;
            Assert.AreEqual(sb.Name, name);
            Assert.AreEqual(sb.Description, desc);
            Assert.AreEqual(sb.Interval, 9);
            Assert.AreEqual(sb.TriggerProbability, SkillTriggerProbability.High);
            Assert.AreEqual(sb.Duration, SkillDuration.Medium);
            Assert.AreEqual(sb.Targets, NoteJudgement.Great | NoteJudgement.Perfect);
            Assert.AreEqual(sb.Rate, 0.17);
        }

        [TestMethod()]
        public void TestCreateComboBonus()
        {
            var name = "スノウライトシャワー";
            var desc = "7秒ごとに中確率で少しの間、COMBOボーナスが15%アップ";
            var skill = Skill.Create(name, desc);
            Assert.IsNotNull(skill);
            Assert.IsInstanceOfType(skill, typeof(Skill.ComboBonus));

            var sb = (Skill.ComboBonus)skill;
            Assert.AreEqual(sb.Name, name);
            Assert.AreEqual(sb.Description, desc);
            Assert.AreEqual(sb.Interval, 7);
            Assert.AreEqual(sb.TriggerProbability, SkillTriggerProbability.Medium);
            Assert.AreEqual(sb.Duration, SkillDuration.Medium);
            Assert.AreEqual(sb.Rate, 0.15);
        }

        [TestMethod()]
        public void TestCreateComboContinuation()
        {
            var name = "はにかみ笑顔";
            var desc = "12秒ごとに中確率でしばらくの間、NICEでもCOMBOが継続する";
            var skill = Skill.Create(name, desc);
            Assert.IsNotNull(skill);
            Assert.IsInstanceOfType(skill, typeof(Skill.ComboContinuation));

            var sb = (Skill.ComboContinuation)skill;
            Assert.AreEqual(sb.Name, name);
            Assert.AreEqual(sb.Description, desc);
            Assert.AreEqual(sb.Interval, 12);
            Assert.AreEqual(sb.TriggerProbability, SkillTriggerProbability.Medium);
            Assert.AreEqual(sb.Duration, SkillDuration.Long);
            Assert.AreEqual(sb.Targets, NoteJudgement.Nice);
        }

        [TestMethod()]
        public void TestCreateLifeRevival()
        {
            var name = "情熱デリバリー";
            var desc = "13秒ごとに高確率でしばらくの間、PERFECTでライフが3回復";
            var skill = Skill.Create(name, desc);
            Assert.IsNotNull(skill);
            Assert.IsInstanceOfType(skill, typeof(Skill.Revival));

            var sb = (Skill.Revival)skill;
            Assert.AreEqual(sb.Name, name);
            Assert.AreEqual(sb.Description, desc);
            Assert.AreEqual(sb.Interval, 13);
            Assert.AreEqual(sb.TriggerProbability, SkillTriggerProbability.High);
            Assert.AreEqual(sb.Duration, SkillDuration.Long);
            Assert.AreEqual(sb.Amount, 3);
        }

        [TestMethod()]
        public void TestCreateLifeGuard()
        {
            var name = "弾ける自分";
            var desc = "11秒ごとに中確率でかなりの間、ライフが減少しなくなる";
            var skill = Skill.Create(name, desc);
            Assert.IsNotNull(skill);
            Assert.IsInstanceOfType(skill, typeof(Skill.DamageGuard));

            var sb = (Skill.DamageGuard)skill;
            Assert.AreEqual(sb.Name, name);
            Assert.AreEqual(sb.Description, desc);
            Assert.AreEqual(sb.Interval, 11);
            Assert.AreEqual(sb.TriggerProbability, SkillTriggerProbability.Medium);
            Assert.AreEqual(sb.Duration, SkillDuration.VeryLong);
        }
    }
}