using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;
using ygBlog.Tools;

namespace ygBlog.ErrorHandles
{
    [Route("Error")]
    public class Error : Controller
    {
        public IActionResult Handle([FromServices]IHostEnvironment hostenv)
        {
            Response.StatusCode = 503;

            return new JsonResult(ApiResponse.Fail("发生了内部错误"));
        }
    }
}
