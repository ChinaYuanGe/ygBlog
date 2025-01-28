using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ygBlog.Managment;

namespace ygBlog.Pages
{
    [Authorize]
    public class artmgrModel : PageModel
    {
        public void OnGet()
        {
            var postMan = new PostManager(Program.db);
            ViewData["groups"] = postMan.GetGroupDatas(Models.PostStatus.Draft);
            ViewData["drafts"] = postMan.GetPosts(0, 100, null, -1, Models.PostStatus.Draft, "=");
            ViewData["count_posts"] = postMan.GetPostCount();
            ViewData["count_drafts"] = postMan.GetPostCount(Models.PostStatus.Draft, "=");
        }
    }
}
