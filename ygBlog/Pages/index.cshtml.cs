using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ygBlog.Models;

namespace ygBlog.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void OnGet([FromQuery(Name = "p")] int page = 0, [FromQuery(Name = "group")] int groupid = -1, [FromQuery(Name = "search")] string? search = "")
        {
            ViewData["query_page"] = page;
            ViewData["query_groupid"] = groupid;
            ViewData["query_search"] = search != null ? search : "";

            var postMan = new Managment.PostManager(Program.db);
            var commMan = new Managment.CommentManager(Program.db);
            var gData = postMan.GetGroupDatas();
            ViewData["groups"] = gData;
            ViewData["groupCount"] = gData.Count();
            ViewData["posts"] = postMan.GetPosts(page, int.Parse(Settings.MainPage.PostOutputLimit.Value), search, groupid);
            ViewData["recent_tags"] = postMan.GetRecentTags();
            long PostCount = postMan.GetPostCount(PostStatus.Pubished, ">=", search, groupid);
            ViewData["count_totalpost"] = PostCount;
            ViewData["count_comments"] = commMan.GetCommentCount(0, CommentVisible.Visible);
            ViewData["count_comments_verf"] = commMan.GetCommentCount(0, CommentVisible.Verifing);

            ViewData["maxpage"] = (int)Math.Ceiling((double)PostCount / double.Parse(Settings.MainPage.PostOutputLimit.Value));
        }
    }
}
