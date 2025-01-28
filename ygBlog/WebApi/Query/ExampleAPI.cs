using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query
{
    [Authorize]
    [Route("api/ex")]
    [ApiController]
    [WebApiExceptionFilter]
    public class GetImageList : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string title)
        {
            throw new NotImplementedException();
        }
    }
}
