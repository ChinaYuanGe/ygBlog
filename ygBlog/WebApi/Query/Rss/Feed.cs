using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Rss
{
    [Route("feed")]
    [ApiController]
    [WebApiExceptionFilter]
    public class Feed : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string items = "";
            foreach (var p in new Managment.PostManager(Program.db).GetPosts(0, 5))
            {
                items += $"""
                    <item>
                        <title>{p.Title}</title>
                        <link>{Tools.HostUrlBuilder.Build(HttpContext,$"/read/{p.Id}")}</link>
                        <guid>{p.TimeLastEdit.Ticks.ToString("X")}</guid>
                        <description>{p.Preview}</description>
                        <pubDate>{p.TimePubished.ToString("R")}</pubDate>
                        <lastBuildDate>{p.TimeLastEdit.ToString("R")}</lastBuildDate>
                    </item>
                    """;
            }
            return new ContentResult()
            {
                Content =
                $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <rss version="2.0">
                    <channel>
                        <title>{Settings.Global.Title}</title>
                        <link>{Tools.HostUrlBuilder.Build(HttpContext)}</link>
                        <description>{Settings.SEO.Description}</description>
                        <language>zh-cn</language>
                        {items}
                    </channel>
                </rss>
                """,
                ContentType = "application/xml",
            };
        }
    }
}
