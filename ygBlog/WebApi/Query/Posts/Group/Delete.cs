using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Posts.Group
{
    [Authorize]
    [Route("api/post/delete_group")]
    [ApiController]
    [WebApiExceptionFilter]
    public class Delete : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] long id)
        {
            var pMan = new Managment.PostManager(Program.db);
            if (pMan.DeleteGroup(id) > 0) return new JsonResult(ApiResponse.Success());
            else return new JsonResult(ApiResponse.Fail("数据库错误或不存在该组"));
        }
    }
}
