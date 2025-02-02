using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ygBlog.Tools
{
    public class HostUrlBuilder
    {
        static string GetPortInfo(HttpContext HttpContext) {
            return $"{(HttpContext.Request.IsHttps ? 
                (HttpContext.Request.Host.Port == 443 ? "" : $":{HttpContext.Request.Host.Port}") :
                (HttpContext.Request.Host.Port == 80 ? "" : $":{HttpContext.Request.Host.Port}"))}";
        }
        public static string Build(HttpContext HttpContext, string append = "")
        {
            return $"{(HttpContext.Request.IsHttps ? "https" : "http")}://{Settings.Other.BindingDomain}{append}";
        }
    }
}
