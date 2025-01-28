using Newtonsoft.Json.Linq;

namespace ygBlog.Models
{
    public enum PostStatus : int
    {
        Hidden = -1,
        Draft = 0,
        Pubished = 1,
        Final = 2
    };
    public class PostData
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long GroupID { get; set; }
        public string GroupName { get; set; }
        public string[] Tags { get; set; }
        public DateTime TimePubished { get; set; }
        public DateTime TimeLastEdit { get; set; }
        public PostStatus Status { get; set; }
        public long CounterVisit { get; set; }
        public long CounterComments { get; set; }
        /* Post's file content */
        public string FileRoot => Path.Combine(FileDir.PostRoot, Id.ToString());
        public string Content { get; set; }
        public string Preview
        {
            get
            {
                string strText = System.Text.RegularExpressions.Regex.Replace(Content, "<[^>]+>", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                if (strText.Length > 30)
                {
                    return strText.Substring(0, 30) + "...";
                }
                else return strText;
            }
        }
        public (string? TitleImage, string[]? UsedImage) Images { get; set; }
    }
}
