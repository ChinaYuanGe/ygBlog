using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.Filters;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Posts.Content
{
    [Authorize]
    [Route("api/post/setstatus")]
    [ApiController]
    [WebApiExceptionFilter]
    public class SetStatus : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] long id, [FromForm] int status)
        {
            var postMan = new Managment.PostManager(Program.db);

            if (Enum.IsDefined(typeof(PostStatus), status))
            {
                if (postMan.SetPostStatus(id, (PostStatus)status) > 0)
                    return new JsonResult(ApiResponse.Success());
                else return new JsonResult(ApiResponse.Fail("数据库错误"));
            }
            else return new JsonResult(ApiResponse.Fail("非法的输入"));
        }
    }
}
