using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Server
{
    [Authorize]
    [Route("api/server/setsetting")]
    [ApiController]
    [WebApiExceptionFilter]
    public class GetImageList : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm(Name = "s")] string settingTarget, [FromForm(Name = "v")]string value)
        {
            string[] setTarget = settingTarget.Split(':');
            if (setTarget.Length != 2) return new JsonResult(ApiResponse.Fail("参数输入有误"));
            string Namespace = setTarget[0];
            string Key = setTarget[1];
            if (Program.db.SetConfig(Key, value, Namespace) > 0)
            {
                return new JsonResult(ApiResponse.Success());
            }
            else return new JsonResult(ApiResponse.Fail("数据未修改."));
        }
    }
}
