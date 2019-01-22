using System.Linq;
using HtmlAgilityPack;

namespace BotHost.Services.Impl
{
    public class HtmlParser : IHtmlParser
    {
        private const string CssStyles = @"body {
            font-family: Tahoma, Verdana, Arial, Helvetica, sans-serif;
            font-size: 14px;
            font-weight: normal;
            padding: 0 0 0 0;
        }
        
        table {
            color: #000000;
            font-size: 24px;
            font-weight: normal;
            padding: 0 0 0 0;
            margin: 5px 5px 5px 5px;
            border-collapse: collapse;
        }
        
        font {
            font-size: 24px;
        }
        
        td {
            width: 600px;
            border-top: none;
            border-bottom: none;
        }
        
        .small_td {
            width: 98px;
        }
        
        table {
            border-right: none;
            border-left: none;
        }
        
        tr:nth-child(2n) {
            background: #F7F7F7; !important
        }
        
        tr:nth-child(2n+1) {
            background: #dcdbdd;
        }
        
        tr:first-child {
            background: #4a76a8;
            color: white;
        }";

        public string ParseDay(string htmlPage, int day)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlPage);
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

            var styles = doc.DocumentNode.SelectSingleNode("//style");
            styles.InnerHtml = CssStyles;
            var outputHtml = doc.DocumentNode.InnerHtml
                .Replace(@"<tr bgcolor=""#ffffff""></tr>", "")
                .Replace(@"<td class=""small_td"" align=""center"">&nbsp;</td><td align=""center"">",
                    @"<td colspan=""2"" align=""center"">");
            outputHtml = Rename(outputHtml);
            return outputHtml;
        }

        public string ParseWeek(string htmlPage)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlPage);
            var styles = doc.DocumentNode.SelectSingleNode("//style");
            styles.InnerHtml = CssStyles;
            return doc.DocumentNode.InnerHtml;
        }

        private string Rename(string input)
        {
            input = input
                .Replace("Учебное здание №14", "2 высотный корпус")
                .Replace("Учебное здание №12", "здание физфака")
                .Replace("Учебное здание №1", "главное здание")
                .Replace("Учебное здание №16", "институт химии")
                .Replace("Учебное здание №18", "АГМУ");
            return input;
        }
    }
}