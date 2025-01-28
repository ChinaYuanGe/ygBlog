using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ygBlog.Managment;
using ygBlog.Models;
using ygBlog.Tools;

namespace ygBlog.WebApi.Query.Posts.Content
{
    [Authorize]
    [Route("api/post/savecontent")]
    [ApiController]
    [WebApiExceptionFilter]
    public class SaveContent : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromForm] long id,
            [FromForm] long group = 0,
            [FromForm] string tags = "",
            [FromForm] string title = "",
            [FromForm] string content = "")
        {
            var pMan = new PostManager(Program.db);
            PostData? p = pMan.GetPost(id);
            if (p != null)
            {
                if (content.Length > 0)
                {
                    p.Content = System.Web.HttpUtility.UrlDecode(Encoding.UTF8.GetString(Convert.FromBase64String(content)));
                }
                else p.Content = "";
                p.Title = title;
                p.GroupID = group;
                p.Tags = tags.Split(',');
                if (pMan.UpdatePost(p) > 0)
                {
                    return new JsonResult(Models.ApiResponse.Success());
                }
                else return new JsonResult(Models.ApiResponse.Fail("数据库错误"));
            }
            else return new JsonResult(Models.ApiResponse.Fail("文章不存在"));
        }
    }
}
