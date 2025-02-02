using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;

namespace ygBlog.WebApi.DynamicResource
{
    [Route("post_img")]
    [Route("img/arts")]
    [Controller]
    [OutputCache(NoStore = false)]
    public class ImageFileController : ControllerBase
    {
        [HttpGet("{FileName}")]
        public async Task<IActionResult> Get([FromRoute] string FileName)
        {
            string path = Path.Combine(FileDir.PostImageRoot, FileName);
            if (!System.IO.File.Exists(path))
            {
                path = Path.Combine(FileDir.WWWRoot, "img", "default_title.jpg");
                if (!System.IO.File.Exists(path)) path = Path.Combine(FileDir.WWWRoot, "img", "default_title.png");
                if (!System.IO.File.Exists(path)) return NotFound();
            }
            var mineProvider = new FileExtensionContentTypeProvider();
            string mime = $"image/{Path.GetExtension(FileName)}";
            mineProvider.TryGetContentType(FileName, out mime);

            System.IO.FileInfo finfo = new FileInfo(path);

            this.HttpContext.Response.Headers.LastModified = finfo.LastWriteTimeUtc.ToString("R");

            byte[] content = System.IO.File.ReadAllBytes(path);
            return File(content, mime);
        }
    }
}
