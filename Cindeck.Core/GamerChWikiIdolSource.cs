using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public class GamerChWikiIdolSource : IIdolSource
    {
        private const int LabelColumn=2, RarityColumn = 3, CategoryColumn = 4, NameColumn = 5, 
            LifeColumn = 12, VocalColumn = 13, DanceColumn = 14, VisualColumn = 15, 
            CenterEffectColumn = 17, CenterEffectDetailsColumn = 18, SkillColumn = 19, SkillDetailsColumn = 20, ImplementationDateColumn=31;

        private IDocumentSource m_doc;

        public GamerChWikiIdolSource(IDocumentSource document)
        {
            m_doc = document;
        }

        private string ExtractLabel(string name)
        {
            var result = Regex.Match(name, "［(.+)］");
            return result.Success ? result.Groups[1].Value : null;
        }

        private Idol ParseIdolData(HtmlNodeCollection td)
        {
            try
            {
                return new Idol(
                ExtractLabel(td[LabelColumn].InnerText.Trim()), td[NameColumn].InnerText.Trim(), td[RarityColumn].InnerText.Trim().ToRarity(),
                td[CategoryColumn].InnerText.Trim().ToIdolCategory(), Convert.ToInt32(td[LifeColumn].InnerText.Trim()), Convert.ToInt32(td[DanceColumn].InnerText.Trim().Replace(",", "")),
                 Convert.ToInt32(td[VocalColumn].InnerText.Trim().Replace(",", "")), Convert.ToInt32(td[VisualColumn].InnerText.Trim().Replace(",", "")),
                 DateTime.Parse(td[ImplementationDateColumn].InnerText.Trim()),
                CenterEffect.Create(td[CenterEffectColumn].InnerText.Trim(), 
                    td[CenterEffectDetailsColumn].InnerText.Trim()
                    .Replace("パッショナイドル", "パッションアイドル")),
                Skill.Create(td[SkillColumn].InnerText.Trim(),
                    td[SkillDetailsColumn].InnerText.Trim()
                    .Replace("PEFECT", "PERFECT")
                    .Replace("PERFCT", "PERFECT")
                    .Replace("秒毎", "秒ごと")
                    .Replace("秒間", "秒ごと")
                    .Replace("しばらく間", "しばらくの間")));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tuple<List<Idol>,int>> GetIdols()
        {
            var raw = await m_doc.Load();
            var hdoc = new HtmlDocument();
            hdoc.LoadHtml(raw);
            var rows = hdoc.DocumentNode.SelectNodes("//article/section/table/tbody/tr");
            var result = new List<Idol>();
            Idol idol;
            foreach(var row in rows)
            {
                idol = ParseIdolData(row.ChildNodes);
                if(idol!=null)
                {
                    result.Add(idol);
                }
            }
            return Tuple.Create(result,rows.Count-result.Count);
        }
    }
}
