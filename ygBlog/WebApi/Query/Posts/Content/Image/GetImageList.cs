using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Posts.Content.Image
{
    [Authorize]
    [Route("api/post/getimagelist")]
    [ApiController]
    [WebApiExceptionFilter]
    public class GetImageList : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] long id)
        {
            var pMan = new Managment.PostManager(Program.db);
            var p = pMan.GetPost(id);
            if (p != null)
            {
                return new JsonResult(ApiResponse.Success(new Dictionary<string, object>
                {
                    { "imgs",p.Images.UsedImage}
                }));
            }
            else return new JsonResult(ApiResponse.Fail("找不到文章"));
        }
    }
}
