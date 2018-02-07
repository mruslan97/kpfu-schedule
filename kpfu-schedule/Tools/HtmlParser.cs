using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace kpfu_schedule.Tools
{
    public class HtmlParser
    {
        public async Task<string> ParseDay(string group, int day)
        {
            var httpClient = new HttpClient();
            //var webClient = new WebClient();
            //var page = webClient.DownloadData($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            var page = await httpClient.GetStringAsync($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
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
            return doc.DocumentNode.InnerHtml.Replace("width:956px;", string.Empty)
                .Replace("td{ width:143px;}", "td{ width:600px;}").Replace("font-size: 10px;", "font-size: 24px; ")
                .Replace("font-size: 13px;", "font-size: 24px;")
                .Replace(".small_td{ width:100px;}", ".small_td{ width:200px;}"); 
        }
    }
}