using System.Text;
using System.Web;

namespace ygBlog.Tools
{
    public class CyBlogOldUnit
    {
        public static class Comment {
            public static string Encode(string rawstring) {

                return Convert.ToBase64String(Encoding.ASCII.GetBytes(Uri.EscapeDataString(rawstring)));
            }
            public static string Decode(string encoded) {
                return Uri.UnescapeDataString(Encoding.ASCII.GetString(Convert.FromBase64String(encoded)));
            }
        }
    }
}
