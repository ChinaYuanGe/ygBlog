using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Posts.Group
{
    [Authorize]
    [Route("api/post/create_group")]
    [ApiController]
    [WebApiExceptionFilter]
    public class Create : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string name)
        {
            var pMan = new Managment.PostManager(Program.db);
            if (!pMan.GroupExists(name))
            {
                if (pMan.CreateGroup(name) > 0)
                {
                    return new JsonResult(ApiResponse.Success(new Dictionary<string, object> {
                    { "id", Program.db.LastInsertRowID},
                    { "name", name}
                }));
                }
                else return new JsonResult(ApiResponse.Fail("数据库错误"));
            }
            else return new JsonResult(ApiResponse.Fail("已经存在该组"));
        }
    }
}
