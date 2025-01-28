using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Posts.Comments
{
    [Route("api/comment/post")]
    [ApiController]
    [WebApiExceptionFilter]
    public class PostComment : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromForm(Name = "a")]long postid,
            [FromForm(Name = "n")]string name,
            [FromForm(Name = "e")]string email,
            [FromForm(Name = "c")]string content,
            [FromForm(Name = "r")]long? repeat = null)
        {

            if (Settings.Comments.Enable.Value == "0")
            {
                return new JsonResult(ApiResponse.Fail("评论功能已关闭"));
            }

#if DEBUG
#else
            Thread.Sleep(500);
#endif

            bool userIsAuth = (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated);
            
            CommentVisible CreateVisibleStatus = CommentVisible.Verifing;

            var cMan = new Managment.CommentManager(Program.db);

            if (Settings.Comments.VerifyEnable.Value == "0" ||
                userIsAuth ||
                cMan.EmailInWhitelist(email))
            {
                CreateVisibleStatus = CommentVisible.Visible;
            }
            else
            {
                if ((cMan.GetCommentCount(0, CommentVisible.Verifing) + 1) > long.Parse(Settings.Comments.VerifyQueueMax.Value))
                {
                    return new JsonResult(ApiResponse.Fail("审核队列已爆满,过会再来试吧"));
                }
            }

            System.Net.Mail.MailAddress? tmp;

            if (System.Net.Mail.MailAddress.TryCreate(email,out tmp) == false) {
                return new JsonResult(ApiResponse.Fail("请输入正确的电子邮箱"));
            }
            if (name.Length < 1)
            {
                name = Settings.Comments.AnonymousName.Value;
            }
            if (content.Length < 4) {
                return new JsonResult(ApiResponse.Fail("请输入更多文字以防止浪费"));
            }

            Comment c = new Comment()
            {
                PostID = postid,
                Name = name,
                Email = email,
                Content = content,
                Respond = repeat != null ? cMan.GetCommentByID((long)repeat) : null,
                Time = DateTime.Now,
                EndPoint = $"{HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}"
            };

            if (cMan.CreateComment(c, CreateVisibleStatus) > 0)
            {
                return new JsonResult(ApiResponse.Success(new Dictionary<string, object>
                {
                    { "checking",CreateVisibleStatus == CommentVisible.Verifing}
                }));
            }
            else return new JsonResult(ApiResponse.Fail("数据库错误"));
        }
    }
}
