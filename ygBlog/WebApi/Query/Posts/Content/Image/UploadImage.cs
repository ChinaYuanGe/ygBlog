using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;
using SixLabors.ImageSharp;
using ygBlog.Tools;
using System.Text;

namespace ygBlog.WebApi.Query.Posts.Content.Image
{
    [Authorize]
    [Route("api/post/upload_image")]
    [ApiController]
    [WebApiExceptionFilter]
    public class UploadImage : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] long id, [FromForm] int type, [FromForm] IFormFile file)
        {
            var pMan = new Managment.PostManager(Program.db);
            var p = pMan.GetPost(id);
            if (p != null)
            {
            regenFilename:
                string targetFilePath = Path.Combine(
                    FileDir.PostImageRoot,
                    $"{Hasher.md5(Encoding.UTF8.GetBytes(new Random().Next(int.MinValue, int.MaxValue).ToString()))}-{Hasher.md5(Encoding.UTF8.GetBytes(new Random().NextDouble().ToString()))}" + ".jpg");
                string targetFilename = Path.GetFileName(targetFilePath);

                if (System.IO.File.Exists(targetFilePath)) goto regenFilename;

                // convert to jpg format.
                try
                {
                    SixLabors.ImageSharp.Image.Load(file.OpenReadStream()).SaveAsJpeg(targetFilePath);
                }
                catch (Exception ex) {
                    return new JsonResult(ApiResponse.Fail($"{ex.Message}"));
                }

                switch (type)
                {
                    case 1: // Title images
                        if (p.Images.TitleImage != null) {
                            if (System.IO.File.Exists(Path.Combine(FileDir.PostImageRoot, p.Images.TitleImage)))
                                System.IO.File.Delete(Path.Combine(FileDir.PostImageRoot, p.Images.TitleImage));
                        }
                        p.Images = (targetFilename, p.Images.UsedImage);
                        break;
                    default:
                    case 0:
                        string[] next = new string[p.Images.UsedImage.Length + 1];
                        Array.Copy(p.Images.UsedImage, next, p.Images.UsedImage.Length);
                        next[next.Length - 1] = targetFilename;
                        p.Images = (p.Images.TitleImage, next);
                        break;
                }
                pMan.UpdatePost(p);
                return new JsonResult(ApiResponse.Success(new Dictionary<string, object> { { "src", $"{Urls.PostImagePath}/{targetFilename}" } }));
            }
            else return new JsonResult(ApiResponse.Fail("找不到文章"));
        }
    }
}
