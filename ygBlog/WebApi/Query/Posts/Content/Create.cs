using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Posts.Content
{
    [Authorize]
    [Route("api/post/create")]
    [ApiController]
    [WebApiExceptionFilter]
    public class Create : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string title) {
            if (title.Length > 0)
            {
                if (new Managment.PostManager(Program.db).CreatePost(title) > 0)
                {
                    return new JsonResult(ApiResponse.Success(new Dictionary<string, object> { { "id", Program.db.LastInsertRowID } }));
                }
                else return Problem("数据库错误, 无法创建");
            }
            else
            {
                return Problem("请输入标题.");
            }
        }
    }
}
