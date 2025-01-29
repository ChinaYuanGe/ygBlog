using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Useful.Tools;
using ygBlog.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ygBlog.Managment
{
    public class PostManager
    {
        Database.Database db { get; set; }
        public PostManager(Database.Database db)
        {
            this.db = db;
        }
        public int CreatePost(string title)
        {
            int dbExeRes = db.Execute($"INSERT INTO posts(`title`,`time_lastedit`) " +
                $"VALUES(@title,@lastedit_time)",
                ("@title", title),
                ("@lastedit_time", new Timestamp(DateTime.Now).timestamp_mill));
            if (dbExeRes > 0)
            {
                long NewID = db.LastInsertRowID;
                string PostRoot = Path.Combine(FileDir.PostRoot, NewID.ToString());
                if (!Directory.Exists(PostRoot)) Directory.CreateDirectory(PostRoot);

                return dbExeRes;
            }
            else return 0;
        }
        public long GetPostCount(PostStatus status = PostStatus.Pubished, string how = ">=", string? filter_search = "", int filter_group = -1)
        {
            return (long)db.QueryValue($"SELECT " +
                $"COUNT(*) " +
                $"FROM posts " +
                $"WHERE " +
                $"status{how}{(int)status} " +
                $"{(filter_search != null ? "AND (title LIKE @search OR tags LIKE @search) " : "")}" +
                $"{(filter_group >= 0 ? $"AND `group`={filter_group}" : "")}",
                ("@search", $"%{filter_search}%"));
        }
        public PostData[] GetPosts(int page, int limit,
            string? filter_search = null,
            int filter_group = -1,
            PostStatus status = PostStatus.Pubished,
            string how = ">=")
        {

            var read = db.Query($"SELECT " +
                $"`id`,`title`,`group`,`tags`,`time_pubish`,`time_lastedit`,`status`,`counter_visit`,`counter_comments`," +
                $"(SELECT `name` FROM `groups` WHERE `groups`.id=`posts`.`group`) as `groupname` " +
                $"FROM posts " +
                $"WHERE " +
                $"status{how}{(int)status} " +
                $"{(filter_search != null ? "AND (title LIKE @search OR tags LIKE @search) " : "")}" +
                $"{(filter_group >= 0 ? "AND `group`=@group " : "")}" +
                $"ORDER BY time_pubish DESC " +
                $"LIMIT {page * limit},{limit}",
                ("@search", $"%{filter_search}%"),
                ("@group", filter_group));

            List<PostData> dat = new List<PostData>();

            while (read.Read())
            {
                var m = new PostData
                {
                    Id = Convert.ToInt64(read["id"]),
                    Title = read["title"].ToString(),
                    GroupID = Convert.ToInt32(read["group"]),
                    GroupName = read["groupname"].GetType() == typeof(DBNull) ? "未分组" : read["groupname"].ToString(),
                    Tags = read["tags"] != DBNull.Value ? read["tags"].ToString().Length > 0 ? read["tags"].ToString().Split(',') : Array.Empty<string>() : Array.Empty<string>(),
                    TimePubished = new Timestamp(Convert.ToInt64(read["time_pubish"])).LocalTime,
                    TimeLastEdit = new Timestamp(Convert.ToInt64(read["time_lastedit"])).LocalTime,
                    Status = (PostStatus)Convert.ToInt32(read["status"]),
                    CounterVisit = Convert.ToInt32(read["counter_visit"]),
                    CounterComments = Convert.ToInt32(read["counter_comments"])
                };
                m.Content = ReadPostContent(m.Id);
                m.Images = ReadPostImageData(m.Id);
                dat.Add(m);
            }
            read.Close();
            return dat.ToArray();
        }
        public int AddPostVisit(long postid)
        {
            return db.Execute($"UPDATE posts SET counter_visit=counter_visit+1 WHERE id={postid}");
        }
        public PostData? GetPost(long postid)
        {
            var read = db.Query($"SELECT " +
                $"`id`,`title`,`group`,`tags`,`time_pubish`,`time_lastedit`,`status`,`counter_visit`,`counter_comments`," +
                $"(SELECT `name` FROM `groups` WHERE `groups`.id=`posts`.`group`) as `groupname` " +
                $"FROM posts " +
                $"WHERE id={postid}");
            if (read.Read())
            {
                var d = new PostData
                {
                    Id = Convert.ToInt64(read["id"]),
                    Title = read["title"].ToString(),
                    GroupID = Convert.ToInt32(read["group"]),
                    GroupName = read["groupname"].ToString(),
                    Tags = read["tags"].ToString().Length > 0 ? read["tags"].ToString().Split(',') : Array.Empty<string>(),
                    TimePubished = new Timestamp(Convert.ToInt64(read["time_pubish"])).LocalTime,
                    TimeLastEdit = new Timestamp(Convert.ToInt64(read["time_lastedit"])).LocalTime,
                    Status = (PostStatus)Convert.ToInt32(read["status"]),
                    CounterVisit = Convert.ToInt32(read["counter_visit"]),
                    CounterComments = Convert.ToInt32(read["counter_comments"]),
                };
                d.Content = ReadPostContent(d.Id);
                d.Images = ReadPostImageData(d.Id);
                d.GroupName = GetGroupName(d.GroupID);
                read.Close();
                return d;
            }
            read.Close();
            return null;
        }
        public string ReadPostContent(long Id)
        {
            string FileRoot = Path.Combine(FileDir.PostRoot, Id.ToString());
            if (File.Exists(Path.Combine(FileRoot, "content.html")))
            {
                return File.ReadAllText(Path.Combine(FileRoot, "content.html"));
            }
            else return "";
        }
        public void WritePostContent(long Id, string content)
        {
            string FileRoot = Path.Combine(FileDir.PostRoot, Id.ToString());
            File.WriteAllText(Path.Combine(FileRoot, "content.html"), content);
        }
        public (string? TitleImage, string[]? UsedImage) ReadPostImageData(long Id)
        {
            string FileRoot = Path.Combine(FileDir.PostRoot, Id.ToString());
            if (!File.Exists(Path.Combine(FileRoot, "images.json")))
            {
                return ("default_title.png", Array.Empty<string>());
            }

            JObject obj = JObject.Parse(File.ReadAllText(Path.Combine(FileRoot, "images.json")));

            return (
                (obj["title_img"].Type != JTokenType.Null && obj["title_img"].ToString().Length > 0 ? obj["title_img"].ToString() : "default_title.png"),
                obj["used_img"].ToObject<string[]>());
        }
        public void WritePostImageData(long Id, (string? TitleImage, string[]? UsedImage) value) {
            string FileRoot = Path.Combine(FileDir.PostRoot, Id.ToString());
            JObject obj = new JObject {
                    { "title_img", value.TitleImage},
                    { "used_img", JArray.FromObject(value.UsedImage)}
                };
            File.WriteAllText(Path.Combine(FileRoot, "images.json"), obj.ToString());
        }
        public int UpdatePost(PostData dat)
        {
            WritePostContent(dat.Id, dat.Content);
            WritePostImageData(dat.Id, dat.Images);
            return db.Execute($"UPDATE posts SET " +
                $"title=@title," +
                $"`group`=@group," +
                $"tags=@tags," +
                $"time_lastedit=@lastedit " +
                $"WHERE id={dat.Id}",
                ("@title", dat.Title),
                ("@group", dat.GroupID),
                ("@tags", string.Join(',', dat.Tags)),
                ("@lastedit", new Timestamp(DateTime.Now).timestamp_mill));
        }
        public string GetGroupName(long gid)
        {
            var read = db.QueryValue($"SELECT name FROM groups WHERE id={gid}");
            if (read == null) return "未分组";
            else return (string)read;
        }
        public int SetPostStatus(long postid, PostStatus status)
        {
            var p = GetPost(postid);
            return db.Execute($"UPDATE posts SET status={(int)status}{((p.Status != PostStatus.Pubished && status == PostStatus.Pubished) ? $",time_pubish={new Timestamp(DateTime.Now).timestamp_mill}" : "")} WHERE id={postid}");
        }
        public int DeletePost(PostData dat)
        {
            return DeletePost(dat);
        }
        public int DeletePost(long id)
        {
            //Delete file first
            var p = GetPost(id);
            if (p != null)
            {
                Directory.Delete(p.FileRoot, true);
                return db.Execute($"DELETE FROM posts WHERE id={id}");
            }
            else throw new ArgumentException("文章不存在");
        }
        /* Group data */
        public (long id, string name, long post_count)[] GetGroupDatas(PostStatus condition = PostStatus.Pubished, string how = ">=")
        {
            var read = db.Query($"SELECT `id`,`name`,(SELECT COUNT(*) FROM `posts` WHERE `posts`.`group`=`groups`.`id` AND `posts`.`status`{how}{(int)condition}) as post_count FROM groups");
            List<(long, string, long)> ret = new List<(long, string, long)>();
            while (read.Read())
            {
                ret.Add((Convert.ToInt64(read["id"]), read["name"].ToString(), Convert.ToInt64(read["post_count"])));
            }
            read.Close();
            return ret.ToArray();
        }
        public long CreateGroup(string name)
        {
            if (db.Execute($"INSERT INTO groups(`name`) VALUES(@name)", ("@name", name)) > 0)
            {
                return db.LastInsertRowID;
            }
            else return 0;
        }
        public long DeleteGroup(long id)
        {
            return db.Execute($"DELETE FROM groups WHERE id={id}");
        }
        public bool GroupExists(string name) {
            return (long)db.QueryValue("SELECT COUNT(*) FROM groups WHERE name=@name", ("@name", name)) > 0;
        }
        /* Tag data */
        public string[] GetRecentTags(int count = 10)
        {
            var read = db.Query($"SELECT tags FROM posts WHERE length(tags)>0 ORDER BY id DESC LIMIT 0,{count}");

            List<string> ret = new List<string>();
            while (read.Read())
            {
                string[] t = read["tags"].ToString().Split(',');
                foreach (var s in t)
                {
                    if (!ret.Contains(s))
                    {
                        ret.Add(s);
                        break;
                    }
                }
            }
            read.Close();
            return ret.ToArray();
        }
        public Dictionary<int, Dictionary<int, List<(long id,string GroupName, string Title)>>> GetArchiveData()
        {
            var read = db.Query($"SELECT " +
                $"`id`,`title`,`time_pubish`," +
                $"(SELECT `name` FROM `groups` WHERE `groups`.id=`posts`.`group`) as `groupname` " +
                $"FROM posts " +
                $"WHERE status>={(int)PostStatus.Pubished} ORDER BY `time_pubish` DESC");

            Dictionary<int, Dictionary<int, List<(long id, string GroupName, string Title)>>> ret = new Dictionary<int, Dictionary<int, List<(long id, string GroupName, string Title)>>>();

            while (read.Read())
            {
                Timestamp t = new Timestamp(Convert.ToInt64(read["time_pubish"]));
                var simplyData = (Convert.ToInt64(read["id"]),(read["groupname"].GetType() != typeof(DBNull) ? read["groupname"].ToString() : "未分组"), read["title"].ToString());
                if (!ret.ContainsKey(t.LocalTime.Year)) ret.Add(t.LocalTime.Year, new Dictionary<int, List<(long id, string GroupName, string Title)>>());
                if (!ret[t.LocalTime.Year].ContainsKey(t.LocalTime.Month)) ret[t.LocalTime.Year].Add(t.LocalTime.Month, new List<(long id, string GroupName, string Title)>());
                ret[t.LocalTime.Year][t.LocalTime.Month].Add(simplyData);
            }
            read.Close();
            return ret;
        }
    }
}