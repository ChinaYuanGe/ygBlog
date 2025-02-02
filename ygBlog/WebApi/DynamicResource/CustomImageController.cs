using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ygBlog.WebApi.DynamicResource
{
    [Route("custom_img")]
    [Controller]
    public class CustomImageController : ControllerBase
    {
        [HttpGet("{FileName}")]
        public async Task<IActionResult> Get([FromRoute] string FileName)
        {
            string path = Path.Combine(FileDir.CustomImageRoot, FileName);
            if (System.IO.File.Exists(path))
            {
                var mineProvider = new FileExtensionContentTypeProvider();
                string mime = $"image/{Path.GetExtension(FileName)}";
                mineProvider.TryGetContentType(FileName, out mime);

                System.IO.FileInfo finfo = new FileInfo(path);

                this.HttpContext.Response.Headers.LastModified = finfo.LastWriteTimeUtc.ToString("R");

                byte[] content = System.IO.File.ReadAllBytes(path);
                return File(content, mime);
            }
            return NotFound();
        }
    }
}
