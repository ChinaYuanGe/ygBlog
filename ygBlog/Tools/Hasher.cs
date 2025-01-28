using System.Security.Cryptography;
using System.Text;

namespace ygBlog.Tools
{
    public class Hasher
    {
        public static string md5(string str) => md5(str, Encoding.UTF8);
        public static string md5(string str, Encoding encode) => md5(encode.GetBytes(str));
        public static string md5(byte[] data) { 
            MD5 md5 = MD5.Create();
            var result = md5.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            foreach (var b in result) {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
        public static string sha256(string str) => sha256(str, Encoding.UTF8);
        public static string sha256(string str, Encoding encode) => sha256(encode.GetBytes(str));
        public static string sha256(byte[] data) { 
            SHA256 sha256 = SHA256.Create();
            var result = sha256.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            foreach (var b in result)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
