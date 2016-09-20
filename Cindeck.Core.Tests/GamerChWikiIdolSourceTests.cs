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
    public class GamerChWikiIdolSourceTests
    {
        [TestMethod()]
        public void TestGetIdols()
        {
            var a= new GamerChWikiIdolSource(new FileDocumentSource("test-wiki-data-page.html")).GetIdols().Result.Item1;
            Assert.AreEqual(a.Count(), 338);
        }
    }
}