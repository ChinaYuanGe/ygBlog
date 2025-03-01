﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using ygBlog.Models;
using ygBlog.Tools;

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
            Thread.Sleep(800); //Anit dos (ddos is not defencible if a single host)
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
                    return new JsonResult(ApiResponse.Fail("审核队列已爆满,过会再来试吧."));
                }
            }

            System.Net.Mail.MailAddress? tmp;

            if (System.Net.Mail.MailAddress.TryCreate(email,out tmp) == false) {
                return new JsonResult(ApiResponse.Fail("请输入正确的电子邮箱."));
            }
            if (name.Length < 1)
            {
                name = Settings.Comments.AnonymousName.Value;
            }
            if (content.Length < 1) {
                return new JsonResult(ApiResponse.Fail("请至少输入一个中文字符或两个英文字符."));
            }

            Comment c = new Comment()
            {
                PostID = postid,
                Name = name,
                Email = email,
                Content = CyBlogOldUnit.Comment.Encode(SecurityUnit.EscapeHtmlArrow(CyBlogOldUnit.Comment.Decode(content))),
                Respond = repeat != null ? cMan.GetCommentByID((long)repeat) : null,
                Time = DateTime.Now,
                EndPoint = $"{HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}"
            };

            if (cMan.CreateComment(c, CreateVisibleStatus) > 0)
            {
                var eMan = new Managment.MailManager(Tools.HostUrlBuilder.Build(HttpContext));
                if (CreateVisibleStatus == CommentVisible.Visible)
                {
                    if (c.Respond != null)// Send email if user is whitelist and reply to someone
                    {
                        c.Content = CyBlogOldUnit.Comment.Decode(c.Content);
                        eMan.SendCommentReplyNotice(c);
                    }
                }
                else if(CreateVisibleStatus == CommentVisible.Verifing)
                {
                    eMan.SendWaitVerifyCommentToManager(c);
                }
                return new JsonResult(ApiResponse.Success(new Dictionary<string, object>
                {
                    { "checking",CreateVisibleStatus == CommentVisible.Verifing}
                }));
            }
            else return new JsonResult(ApiResponse.Fail("数据库错误"));
        }
    }
}
