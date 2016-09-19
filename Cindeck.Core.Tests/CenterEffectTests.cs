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
    public class CenterEffectTests
    {
        [TestMethod()]
        public void TestCreateAppealUpEffect()
        {
            var name = "パッションステップ";
            var desc = "パッションアイドルのダンスアピール値90%アップ";
            var effect = CenterEffect.Create(name, desc);
            Assert.IsNotNull(effect);
            Assert.IsInstanceOfType(effect, typeof(CenterEffect.AppealUp));

            var sb = (CenterEffect.AppealUp)effect;
            Assert.AreEqual(sb.Name, name);
            Assert.AreEqual(sb.Description, desc);
            Assert.AreEqual(sb.Rate, 0.9);
            Assert.AreEqual(sb.Targets, IdolCategory.Passion);
            Assert.AreEqual(sb.TargetAppeal, AppealType.Dance);
        }

        [TestMethod()]
        public void TestCreateSkillTriggerProbabilityEffect()
        {
            var name = "パッションアビリティ";
            var desc = "パッションアイドルの特技発動確率30%アップ";
            var effect = CenterEffect.Create(name, desc);
            Assert.IsNotNull(effect);
            Assert.IsInstanceOfType(effect, typeof(CenterEffect.SkillTriggerProbabilityUp));

            var sb = (CenterEffect.SkillTriggerProbabilityUp)effect;
            Assert.AreEqual(sb.Name, name);
            Assert.AreEqual(sb.Description, desc);
            Assert.AreEqual(sb.Rate, 0.3);
            Assert.AreEqual(sb.Targets, IdolCategory.Passion);
        }
    }
}