using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ygBlog.Models;

namespace ygBlog.Pages
{
    public class readModel : PageModel
    {
        [HttpGet("read/{id}")]
        public void OnGet([FromRoute]int id = -1)
        {
            if (id >= 0)
            {
                var pMan = new Managment.PostManager(Program.db);
                PostData? read = pMan.GetPost(id);
                if (read != null && (int)read.Status > 0)
                {
                    ViewData["post"] = (PostData)read;
                    pMan.AddPostVisit(read.Id);
                }
                else NotFound();
            }
            else NotFound();
        }
    }
}
