using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ygBlog.Pages
{
    public class UserErrorModel : PageModel
    {
        public void OnGet([FromQuery]string? code)
        {
            ViewData["code"] = code;
        }
    }
}
