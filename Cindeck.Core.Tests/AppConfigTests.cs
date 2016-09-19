using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cindeck.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cindeck.Core.Tests
{
    [TestClass()]
    public class AppConfigTests
    {
        [TestMethod()]
        public void TestSave()
        {
            var cfg = AppConfig.Load();
            //cfg.IdolPool = new GamerChWikiIdolSource(new FileDocumentSource("test-wiki-data-page.html")).GetIdols().Result.ToList();
            try
            {
                cfg.Save();
                var loaded = AppConfig.Load();
                Assert.AreEqual(loaded.ImplementedIdols.Count, cfg.ImplementedIdols.Count);
            }
            finally
            {
                AppConfig.Reset();
            }
        }
    }
}