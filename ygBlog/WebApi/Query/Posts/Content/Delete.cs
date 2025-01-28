using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ygBlog.WebApi.Query.Posts.Content
{
    [Authorize]
    [Route("api/post/delete")]
    [ApiController]
    [WebApiExceptionFilter]
    public class Delete : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] long id)
        {
            if (new Managment.PostManager(Program.db).DeletePost(id) > 0)
            {
                return new JsonResult(Models.ApiResponse.Success());
            }
            else return new JsonResult(Models.ApiResponse.Fail("数据库错误."));
        }
    }
}
