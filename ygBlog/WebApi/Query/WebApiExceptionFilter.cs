using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using ygBlog.Models;

namespace ygBlog.WebApi.Query
{
    public class WebApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = new JsonResult(ApiResponse.Fatal(context.Exception));
        }
    }
}
