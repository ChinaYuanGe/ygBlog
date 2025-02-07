namespace ygBlog.Tools
{
    public class SecurityUnit
    {
        public static string EscapeHtmlArrow(string rawInput)
        {
            return rawInput
                .Replace("<","&lt;")
                .Replace(">","&gt;");
        }
    }
}
