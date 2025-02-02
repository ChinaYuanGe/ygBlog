using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ygBlog.Pages
{
    public class DEBUGModel : PageModel
    {
        public void OnGet()
        {
#if DEBUG
#else
            NotFound();
#endif
        }
    }
}
