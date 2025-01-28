using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using Useful.Tools;
using ygBlog.Models;

namespace ygBlog.Managment
{
    public class CommentManager
    {
        Database.Database db { get; set; }
        public CommentManager(Database.Database db)
        {
            this.db = db;
        }
        public Comment[] GetComments(int page, int limit, long postid = -1, CommentVisible? condition = CommentVisible.Visible, string how = "=")
        {
            var read = db.Query($"SELECT * FROM comments{(postid >= 0 ? $" WHERE artid={postid}" : "")}{(condition != null ? $" {(postid >= 0 ? " AND" : " WHERE")} visible{how}{(int)condition}" : "")} LIMIT {page * limit},{limit}");

            List<Comment> ret = new List<Comment>();

            while (read.Read())
            {
                ret.Add(new Comment()
                {
                    Id = Convert.ToInt64(read["id"]),
                    Name = read["name"].ToString(),
                    Email = read["email"].ToString(),
                    Content = HttpUtility.UrlDecode(Encoding.ASCII.GetString(Convert.FromBase64String(read["content"].ToString()))),
                    PostID = Convert.ToInt64(read["artid"]),
                    Respond = GetCommentByID(Convert.ToInt64(read["resp"]), condition == CommentVisible.Visible),
                    Visible = (CommentVisible)Convert.ToInt32(read["visible"]),
                    Time = new Timestamp(Convert.ToInt64(read["time"])).LocalTime,
                    EndPoint = read["endpoint"].ToString()
                });
            }
            read.Close();
            return ret.ToArray();
        }
        public Comment? GetCommentByID(long id, bool onlyVisible = true)
        {
            if (id <= 0) return null;
            var read = db.Query($"SELECT * FROM comments WHERE id={id}{(onlyVisible ? " AND visible=1" : "")}");
            List<Comment> ret = new List<Comment>();

            if (read.Read())
            {
                var res = new Comment()
                {
                    Id = Convert.ToInt64(read["id"]),
                    Name = read["name"].ToString(),
                    Email = read["email"].ToString(),
                    Content = Uri.EscapeDataString(Encoding.ASCII.GetString(Convert.FromBase64String(read["content"].ToString()))),
                    PostID = Convert.ToInt64(read["artid"]),
                    Respond = GetCommentByID(Convert.ToInt64(read["resp"])),
                    Visible = (CommentVisible)Convert.ToInt32(read["visible"]),
                    Time = new Timestamp(Convert.ToInt64(read["time"])).LocalTime,
                    EndPoint = read["endpoint"].ToString()
                };
                read.Close();
                return res;
            }
            read.Close();
            return null;
        }
        public int CreateComment(Comment c, CommentVisible visible = CommentVisible.Verifing)
        {
            if (visible == CommentVisible.Visible)
            {
                if (db.Execute($"UPDATE posts SET counter_comments=counter_comments+1 WHERE id={c.PostID}") <= 0) {
                    return 0;
                }
            }

            return db.Execute($"INSERT INTO comments(`name`,`email`,`content`,`artid`,`resp`,`time`,`endpoint`,`visible`) " +
                $"VALUES(@name,@email,@content,@artid,@resp,@time,@endpoint,@vis)",
                ("@name", c.Name),
                ("@email", c.Email),
                ("@content", c.Content),
                ("@artid", c.PostID),
                ("@resp", (c.Respond != null ? c.Respond.Id : 0)),
                ("@time", new Timestamp(c.Time).timestamp_mill),
                ("@endpoint", c.EndPoint),
                ("@vis", (int)visible));
        }
        public int SetCommentVisible(Comment c, CommentVisible visible = CommentVisible.Visible)
        {
            return SetCommentVisible(c.Id, visible);
        }
        public int SetCommentVisible(long id, CommentVisible visible = CommentVisible.Visible)
        {
            int ret = 0;
            var org = GetCommentByID(id, false);
            if (db.Execute($"UPDATE comments SET visible={(int)visible} WHERE id={id}") > 0)
            {
                ret++;
                if (org.Visible != CommentVisible.Visible && visible == CommentVisible.Visible)
                {
                    if (db.Execute($"UPDATE posts SET counter_comments=counter_comments+1 WHERE id={org.PostID}") > 0)
                    {
                        ret++;
                    }
                }
                else if(org.Visible == CommentVisible.Visible && visible != CommentVisible.Visible)
                {
                    if (db.Execute($"UPDATE posts SET counter_comments=counter_comments-1 WHERE id={org.PostID}") > 0)
                    {
                        ret++;
                    }
                }
            }
            return ret;
        }
        public long GetCommentCount(long postid = 0, CommentVisible? visible = null)
        {
            if (postid > 0)
            {
                if (visible == null)
                {
                    return (long)db.QueryValue($"SELECT COUNT(*) FROM comments WHERE artid={postid}");
                }
                else if (visible == CommentVisible.Visible)
                {
                    return (long)db.QueryValue($"SELECT counter_comments FROM posts WHERE id={postid}");
                }
                else {
                    return (long)db.QueryValue($"SELECT COUNT(*) FROM comments WHERE artid={postid} AND visible={(int)visible}");
                }
            }
            else
            {
                if (visible == null)
                {
                    return (long)db.QueryValue("SELECT COUNT(*) FROM comments");
                }
                else if (visible == CommentVisible.Visible)
                {
                    var val = db.QueryValue("SELECT SUM(counter_comments) FROM posts");
                    if (val != null) return (long)val;
                    else return 0;
                }
                else
                {
                    return (long)db.QueryValue($"SELECT COUNT(*) FROM comments WHERE visible={(int)visible}");
                }
            }
        }
        /* Whitelist */
        public int CreateWhitelist(string email) { 
            return db.Execute("INSERT INTO comment_whitelist(`email`) VALUES(@email)",
                ("@email", email));
        }
        public int DeleteWhitelist(long id) {
            return db.Execute("DELETE FROM comment_whitelist WHERE id=@id",
                ("@id", id));
        }
        public (long, string)[] GetWhitelists() {
            var read = db.Query("SELECT * FROM comment_whitelist");
            List<(long, string)> ret = new List<(long, string)>();
            while (read.Read())
            {
                ret.Add((Convert.ToInt64(read["id"]), read["email"].ToString()));
            }
            read.Close();
            return ret.ToArray();
        }
        public bool EmailInWhitelist(string email)
        {
            return false;
        }
    }
}