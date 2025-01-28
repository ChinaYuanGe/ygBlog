using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ygBlog.Pages
{
    [Authorize]
    public class commentmgrModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
