using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;
using ygBlog.Tools;

namespace ygBlog.ErrorHandles
{
    [Route("/DevError")]
    public class DevError : Controller
    {
        public IActionResult Handle([FromServices] IHostEnvironment hostenv)
        {
            Response.StatusCode = 503;
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return new JsonResult(ApiResponse.Fatal(exceptionHandlerFeature.Error));
        }
    }
}
