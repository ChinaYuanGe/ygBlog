using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ygBlog.Models
{
    public class ApiResponse
    {
        public int status { get; set; }
        public Dictionary<string, object>? data { get; set; }
        public ApiResponse(int status, Dictionary<string, object>? data){
            this.status = status;
            this.data = data;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static ApiResponse Success(Dictionary<string, object>? data = null)
        {
            return new ApiResponse(200, data);
        }
        public static ApiResponse Fail(string message) {
            return ApiResponse.Error(message, 403);
        }
        public static ApiResponse Error(string message,int code = 500)
        {
            return new ApiResponse(code, new Dictionary<string, object> { { "msg", message } });
        }
        public static ApiResponse Fatal(Exception ex) {
            return new ApiResponse(500, new Dictionary<string, object>
            {
                { "msg",ex.Message},
                { "type",ex.GetType()},
                { "trace",ex.StackTrace != null ? ex.StackTrace : "empty"},
            });
        }
    }
}
