using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ygBlog.Pages
{
    [Authorize]
    public class editModel : PageModel
    {
        [HttpGet("{id}")]
        public void OnGet([FromRoute]int id)
        {
            var postMan = new Managment.PostManager(Program.db);
            ViewData["post"] = postMan.GetPost(id);
            ViewData["groups"] = postMan.GetGroupDatas();
        }
    }
}
