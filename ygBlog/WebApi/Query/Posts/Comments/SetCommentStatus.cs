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
            var eMan = new Managment.MailManager(Tools.HostUrlBuilder.Build(HttpContext));
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
                            if ((CommentVisible)status == CommentVisible.Visible && !eMan.SendCommentPassAndWhiteListNotice(p)) {
                                return new JsonResult(ApiResponse.Fail("无法发送邮件, 但白名单已设置, 并且评论已通过"));
                            }
                            return new JsonResult(ApiResponse.Success());
                        }
                        else return new JsonResult(ApiResponse.Fail("无法设置白名单, 但评论已设置"));
                    }
                    else
                    {
                        if ((CommentVisible)status == CommentVisible.Visible && !eMan.SendCommentPassNotice(p)) {
                            return new JsonResult(ApiResponse.Fail("无法发送邮件, 但已完成状态设定"));
                        }
                    }
                    return new JsonResult(ApiResponse.Success());
                }
                else return new JsonResult(ApiResponse.Fail("数据库错误"));
            }
            else return new JsonResult(ApiResponse.Fail("无法找到评论"));
        }
    }
}
