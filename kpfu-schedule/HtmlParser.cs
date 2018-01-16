using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace kpfu_schedule
{
    public class HtmlParser
    {
        public string GetDay(string group, int day)
        {
            var webClient = new WebClient();
            var page = webClient.DownloadString($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            var trNodes = doc.DocumentNode.SelectSingleNode("//table").ChildNodes.Where(x => x.Name == "tr");
            foreach (var row in trNodes)
            {
                var tmp = new HtmlNodeCollection(row);
                tmp.Clear();
                var tdNodes = row.ChildNodes.Where(x => x.Name == "td").ToArray();
                if (tdNodes.Count() != 0)
                {
                    tmp.Add(tdNodes[0]);
                    tmp.Add(tdNodes[day]);
                }
                row.ChildNodes.Clear();
                if (!tmp[1].InnerHtml.Contains("&nbsp;"))
                {
                    row.ChildNodes.Add(tmp[0]);
                    row.ChildNodes.Add(tmp[1]);
                }
            }
            return doc.DocumentNode.InnerHtml.Replace("width:956px;",String.Empty).Replace("td{ width:143px;}", "td{ width:200px;}");
        }
    }
}
