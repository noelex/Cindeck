using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public class WebDocumentSource : IDocumentSource
    {
        private string m_url;

        public WebDocumentSource(string url)
        {
            m_url = url;
        }

        public async Task<string> Load()
        {
            using (var client = new WebClient())
            {
                var raw= await client.DownloadDataTaskAsync(m_url);
                return Encoding.UTF8.GetString(raw);
            }
        }
    }
}
