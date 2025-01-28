using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ygBlog.Models;

namespace ygBlog.WebApi.Query.Other
{
    [Route("api/auth")]
    [ApiController]
    [WebApiExceptionFilter]
    public class Auth : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string passmd5)
        {
#if DEBUG

#else
            Thread.Sleep(500); //Anti brute-force
#endif

            if (Settings.Global.Password.Value != null)
            {
                if (passmd5.ToLower() == Settings.Global.Password.Value.ToLower()) {
                    RegisterLogin();
                    return new JsonResult(ApiResponse.Success());
                }
            }
            else
            {
                Settings.Global.Password.Value = passmd5;
                RegisterLogin();
                return new JsonResult(ApiResponse.Success());
            }

            return new JsonResult(ApiResponse.Fail("密码错误"));
        }
        void RegisterLogin()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties() { ExpiresUtc = DateTime.Now.AddDays(1) });
        }
    }
}
