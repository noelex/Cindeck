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
    public class GamerChWikiSongSourceTests
    {
        [TestMethod()]
        public void TestGetSongs()
        {
            var a = new GamerChWikiSongSource(new FileDocumentSource("test-song-data-page.html")).GetSongs().Result.Item1;
            Assert.AreEqual(a.Count(), 338);
        }
    }
}