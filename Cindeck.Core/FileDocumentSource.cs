using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public class FileDocumentSource : IDocumentSource
    {
        private string m_path;

        public FileDocumentSource(string path)
        {
            m_path = path;
        }

        public async Task<string> Load()
        {
            return File.ReadAllText(m_path);
        }
    }
}
