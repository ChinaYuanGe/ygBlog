using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace ygBlog.WebApi.Query.Posts.Comments
{
    [Route("comment/avator/{hash}")]
    [ApiController]
    [WebApiExceptionFilter]
    public class GetAvator : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string hash)
        {
            string cachePath = Path.Combine(FileDir.CacheAvatarRoot, $"{hash}.png");

            bool downloadNeeded =  
                !System.IO.File.Exists(cachePath) || 
                new FileInfo(cachePath).LastWriteTime.AddSeconds(int.Parse(Settings.AvatarService.CacheLifetime.Value)) < DateTime.Now;

            if (downloadNeeded)
            {
                HttpClient c = new HttpClient();
                c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ygBlog", "1.0"));
                var t = c.GetAsync(string.Format(Settings.AvatarService.Url.Value, hash));
                t.Wait(5000);
                if (t.IsCompleted)
                {
                    if (t.Result.IsSuccessStatusCode)
                    {
                        var stream = t.Result.Content.ReadAsStream();
                        if (System.IO.File.Exists(cachePath)) {
                            System.IO.File.Delete(cachePath);
                        }
                        FileStream fs = System.IO.File.Create(cachePath);
                        stream.CopyTo(fs, 1024);
                        fs.Flush();
                        fs.Close();

                        // Delete the most old cache

                        var files = Directory.GetFiles(FileDir.CacheAvatarRoot);

                        if (files.Count() > int.Parse(Settings.AvatarService.CacheLimit.Value))
                        {
                            System.IO.File.Delete(
                                files.OrderBy(x => new FileInfo(x).LastWriteTime).First());
                        }
                    }
                }
            }

            if (System.IO.File.Exists(cachePath))
            {
                return File(System.IO.File.ReadAllBytes(cachePath), "image/png");
            }
            else
            {
                if (System.IO.File.Exists(Path.Combine(FileDir.CustomImageRoot, "comment_def_avator.jpg")))
                    return File(System.IO.File.ReadAllBytes(Path.Combine(FileDir.CustomImageRoot, "comment_def_avator.jpg")), "image/jpeg");
                else if (System.IO.File.Exists(Path.Combine(FileDir.CustomImageRoot, "comment_def_avator.png")))
                    return File(System.IO.File.ReadAllBytes(Path.Combine(FileDir.CustomImageRoot, "comment_def_avator.png")), "image/png");
                else
                    return NotFound();
            }
        }
    }
}
