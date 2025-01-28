using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Posts.Comments
{
    [Authorize]
    [Route("api/comment/setstatus")]
    [ApiController]
    [WebApiExceptionFilter]
    public class GetImageList : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] long id, [FromForm]int status, [FromForm(Name = "white")]bool MakeWhiteList)
        {
            var cMan = new Managment.CommentManager(Program.db);
            var p = cMan.GetCommentByID(id, false);
            if (p != null)
            {
                if (cMan.SetCommentVisible(p, (CommentVisible)status) > 0)
                {
                    if (MakeWhiteList)
                    {
                        if (cMan.CreateWhitelist(p.Email) > 0)
                        {
                            return new JsonResult(ApiResponse.Success());
                        }
                        else return new JsonResult(ApiResponse.Fail("无法设置白名单, 但评论已设置"));
                    }
                    return new JsonResult(ApiResponse.Success());
                }
                else return new JsonResult(ApiResponse.Fail("数据库错误"));
            }
            else return new JsonResult(ApiResponse.Fail("无法找到评论"));
        }
    }
}
