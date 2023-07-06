using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace XmlRender.Pages
{
    public class IndexModel : PageModel
    {
        public List<RssItem> RssItems { get; set; }

        public async Task OnGetAsync()
        {
            var rssUrl = "http://scripting.com/rss.xml";
            var rssItems = new List<RssItem>();

            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.GetAsync(rssUrl);
                var xmlString = await response.Content.ReadAsStringAsync();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                foreach (XmlNode itemNode in xmlDoc.SelectNodes("rss/channel/item"))
                {
                    var title = itemNode.SelectSingleNode("title")?.InnerText;
                    var link = itemNode.SelectSingleNode("link")?.InnerText;
                    var description = itemNode.SelectSingleNode("description")?.InnerText;
                    var pubDate = DateTime.TryParse(itemNode.SelectSingleNode("pubDate")?.InnerText, out var parsedDate)
                        ? parsedDate
                        : DateTime.MinValue;

                    rssItems.Add(new RssItem(title, link, description, pubDate));
                }
            }

            RssItems = rssItems;
        }
    }

    public class RssItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PubDate { get; set; }

        public RssItem(string title, string link, string description, DateTime pubDate)
        {
            Title = title;
            Link = link;
            Description = description;
            PubDate = pubDate;
        }
    }
}