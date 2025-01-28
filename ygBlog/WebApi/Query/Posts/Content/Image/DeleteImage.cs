using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Posts.Content.Image
{
    [Authorize]
    [Route("api/post/deleteimage")]
    [ApiController]
    [WebApiExceptionFilter]
    public class DeleteImage : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] long id, [FromForm] string pic)
        {
            var pMan = new Managment.PostManager(Program.db);
            var p = pMan.GetPost(id);
            if (p != null)
            {
                if (p.Images.UsedImage.Contains(pic)) {
                    var New = p.Images.UsedImage.ToList();
                    New.Remove(pic);
                    p.Images = (p.Images.TitleImage, New.ToArray());
                    pMan.UpdatePost(p);
                    return new JsonResult(ApiResponse.Success());
                }
                else return new JsonResult(ApiResponse.Fail("没有该图片"));
            }
            else return new JsonResult(ApiResponse.Fail("找不到文章"));
        }
    }
}
