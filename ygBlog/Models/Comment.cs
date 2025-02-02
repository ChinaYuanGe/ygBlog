using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web;
using ygBlog.Tools;

namespace ygBlog.Models
{
    public enum CommentVisible : int { 
        Hidden = -1,
        Verifing = 0,
        Visible = 1
    };
    public class Comment
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
        [JsonProperty(PropertyName = "artid")]
        public long PostID { get; set; }
        [JsonIgnore]
        public PostData PostData => new Managment.PostManager(Program.db).GetPost(PostID);
        [JsonIgnore]
        public Comment? Respond { get; set; }
        [JsonProperty(PropertyName = "respid")]
        public long RespondCommentID
        {
            get
            {
                if (Respond == null) return 0;
                return Respond.Id;
            }
        }
        [JsonProperty(PropertyName = "repname")]
        public string ResponderName
        {
            get
            {
                if (Respond == null) return "";
                return Respond.Name;
            }
        }
        [JsonProperty(PropertyName = "repsrc")]
        public string ResponderContent
        {
            get
            {
                if (Respond == null) return "";
                return CyBlogOldUnit.Comment.Encode(Respond.Content);
            }
        }
        [JsonIgnore]
        public CommentVisible Visible { get; set; }
        [JsonProperty("visible")]
        public int VisibleInt => (int)Visible;
        [JsonIgnore]
        public DateTime Time { get; set; }

        // in cyblog2 is formatted str
        [JsonProperty("time")]
        public string Timestamp => Time.ToString("yyyy-MM-dd HH:mm:ss");
        [JsonProperty("endpoint")]
        public string EndPoint { get; set; }
    }
}
