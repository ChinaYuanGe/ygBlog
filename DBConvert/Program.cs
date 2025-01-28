using MySql.Data;
using Microsoft.Data.Sqlite;
using System.Net;
using Useful.Tools;
using ygBlog;
using Org.BouncyCastle.Asn1;
using SixLabors.ImageSharp;

namespace DBConvert
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ygBlog.Database.Database dest = new ygBlog.Database.Database("data.db");
            MySqlDB src = new MySqlDB(new System.Net.IPEndPoint(IPAddress.Parse("192.168.17.181"), 3306), "chinayuange", "Chinayuange2003", "blog2");

            // Groups
            var r2 = src.Query("SELECT * FROM groups");
            while (r2.Read()) {
                dest.Execute($"INSERT INTO groups(`id`,`name`) VALUES({Convert.ToInt32(r2["id"])},'{r2["name"].ToString()}')");
            }
            r2.Close();
            Console.WriteLine("Finished group table.");

            // Posts
            var r1 = src.Query("SELECT * FROM arts");
            while (r1.Read()) {
                dest.Execute($"INSERT INTO posts" +
                    $"(`id`,`title`,`group`,`tags`,`time_pubish`,`time_lastedit`,`status`,`counter_visit`,`counter_comments`) " +
                    $"VALUES" +
                    $"(" +
                    $"{Convert.ToInt32(r1["id"])}," +
                    $"'{r1["title"].ToString()}'," +
                    $"{Convert.ToInt32(r1["group"])}," +
                    $"'{r1["tags"].ToString()}'," +
                    $"{new Timestamp(DateTime.Parse(r1["time_pubish"].ToString())).timestamp_mill}," +
                    $"{new Timestamp(DateTime.Parse(r1["time_lastedit"].ToString())).timestamp_mill}," +
                    $"{Convert.ToInt32(r1["status"])}," +
                    $"{Convert.ToInt32(r1["counter_visit"])}," +
                    $"{Convert.ToInt32(r1["counter_commits"])}" +
                    $")");
            }
            r1.Close();

            var r3 = src.Query($"SELECT * FROM comment");

            while (r3.Read()) {
                dest.Execute($"INSERT INTO comments" +
                    $"(`id`,`name`,`email`,`content`,`artid`,`resp`,`visible`,`time`,`endpoint`) " +
                    $"VALUES" +
                    $"(" +
                    $"{Convert.ToInt32(r3["id"])}," +
                    $"'{r3["name"].ToString()}'," +
                    $"'{r3["email"].ToString()}'," +
                    $"'{r3["content"].ToString()}'," +
                    $"{Convert.ToInt32(r3["artid"])}," +
                    $"{Convert.ToInt32(r3["resp"])}," +
                    $"{Convert.ToInt32(r3["visible"])}," +
                    $"{new Timestamp(DateTime.Parse(r3["time"].ToString())).timestamp_mill}," +
                    $"'0.0.0.0:0'" +
                    $")");
            }
            r3.Close();



            // Image convert
            ygBlog.Managment.PostManager pMan = new ygBlog.Managment.PostManager(dest);

            var posts = pMan.GetPosts(0, 10000, status: ygBlog.Models.PostStatus.Hidden);
            foreach (var p in posts)
            {
                List<string> imagestrList = new List<string>();
                imagestrList.Add(p.Images.TitleImage);
                imagestrList.AddRange(p.Images.UsedImage);
                foreach (var a in imagestrList)
                {
                    if (a != null)
                    {
                        string fPath = Path.Combine(ygBlog.FileDir.PostImageRoot, a);
                        if (File.Exists(fPath))
                        {
                            try
                            {
                                MemoryStream ms = new MemoryStream();
                                FileStream fs = File.Open(fPath, FileMode.Open);
                                fs.CopyTo(ms);
                                fs.Close();
                                SixLabors.ImageSharp.Image i = SixLabors.ImageSharp.Image.Load(ms);
                                File.Delete(fPath);
                                i.SaveAsJpeg(fPath);
                                ms.Close();
                            }
                            catch (Exception ex) {
                                Console.WriteLine($"Abort convert \"{fPath}\" cause:{ex.Message}");
                            }
                        }
                    }
                }
            }
        }
    }
}
