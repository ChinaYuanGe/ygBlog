using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web;
using ygBlog.Models;
using ygBlog.Tools;

namespace ygBlog.WebApi.Query.Posts.Comments
{
    [Route("api/comments/get")]
    [ApiController]
    [WebApiExceptionFilter]
    public class GetComment : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int page, [FromQuery] int postid = -1, [FromQuery(Name = "ss")]bool showsensity = false, [FromQuery(Name = "h")]int visibleType = (int)CommentVisible.Visible)
        {
            if (Settings.Comments.Enable.Value == "0")
            {
                return new JsonResult(new ApiResponse(201, null));
            }

            bool canShowSensity = (showsensity && (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated));

            var cMan = new Managment.CommentManager(Program.db);
            int limits = int.Parse(Settings.Comments.OutputLimit.Value);

            var acturalVisibleType = (visibleType != (int)CommentVisible.Visible ?
                (canShowSensity ? (CommentVisible)visibleType : CommentVisible.Visible) :
                CommentVisible.Visible);

            var comments = cMan.GetComments((int)page, limits, postid, acturalVisibleType);

            var count = cMan.GetCommentCount(postid, acturalVisibleType);

            foreach (var item in comments) {
                // Never show these sensity data if client isn't admin
                if (!canShowSensity)
                {
                    item.Email = Hasher.md5(item.Email).ToLower(); // use for getavatar
                    item.EndPoint = "__HIDDEN__";
                }

                // Convert to client can understanded
                item.Content = Convert.ToBase64String(Encoding.ASCII.GetBytes(Uri.EscapeDataString(item.Content)));
            }
            return new JsonResult(ApiResponse.Success(new Dictionary<string, object>
            {
                { "maxPage",(int)Math.Ceiling((double)count / (double)limits)},
                { "count",count},
                { "commits",JArray.FromObject(comments)}
            }));
        }
    }
}
