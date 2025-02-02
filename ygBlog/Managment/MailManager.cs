using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Diagnostics;
using ygBlog.Models;

namespace ygBlog.Managment
{
    public class MailManager
    {
        public readonly static string[] VerifiedEmailService = new string[] {
            "qq.com",
            "163.com",
            "126.com",
            "gmail.com",
            "outlook.com",
            "live.com"};

        private string thishostUrl { get; set; }
        public MailManager(string thisHostUrl) {
            thishostUrl = thisHostUrl;
        }
        public bool SendMail(string to, string who, string subject, string HtmlContent)
        {
            if (Settings.SMTP.Enable.Value != "1") return false;

#if DEBUG
            Console.WriteLine($"""
                ==========START OF EMAIL==========
                To:{to},
                Who:{who},
                Subject:{subject},
                Content:
                {HtmlContent}
                ===========END OF EMAIL===========
                """);

            return true;
#else
            try
            {
                SmtpClient c = new SmtpClient();
                c.Connect(Settings.SMTP.Server.Value, int.Parse(Settings.SMTP.ServerPort.Value), SecureSocketOptions.Auto);
                c.Authenticate(Settings.SMTP.Auth_User.Value, Settings.SMTP.Auth_Pass.Value);

                var msg = new MimeKit.MimeMessage();
                msg.From.Add(new MimeKit.MailboxAddress(Settings.SMTP.SenderName.Value, Settings.SMTP.SenderMail.Value));
                msg.To.Add(new MimeKit.MailboxAddress(who, to));

                msg.Subject = subject;
                msg.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = HtmlContent };

                c.Send(msg);

                c.Disconnect(true);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Send Email error:{ex.Message}\n{ex.StackTrace}");
            }
            return false;
#endif
        }
        /*
            If Email is first time to comment, send a email to notice user.
         */
        public bool SendCommentPassNotice(Comment c)
        {
            string Content = $"""
            <h1>{c.Name}, 感谢您在 {Settings.Global.Title.Value} 上进行评论.</h1>
            <hr/>
            <p>您之所以收到这封邮件, 是因为您在文章<a href="{thishostUrl}/read/{c.PostID}">《{c.PostData.Title}》</a>上的评论已通过审核.</p>
            <p>当您的评论被回复并通过审核时, 您将会收到相关邮件.</p>
            <p>您的评论内容如下</p>
            <p></p>{(c.Respond != null ? $"<p>@{c.Respond.Name}:{c.Respond.Content}</p>":"")}
            <p>{c.Content}</p>
            <hr/>
            <p>谢谢您的评论</p>
            <p>站长 {Settings.Global.Hoster.Value}</p>
            """;

            return SendMail(c.Email, c.Name, $"您的评论已通过审核!", Content);
        }
        public bool SendCommentPassAndWhiteListNotice(Comment c)
        {
            string Content = $"""
            <h1>感谢您在 {Settings.Global.Title.Value} 上进行评论.</h1>
            <hr/>
            <p>您之所以收到这封邮件, 是因为您在文章<a href="{thishostUrl}/read/{c.PostID}">《{c.PostData.Title}》</a>上的评论已通过审核,</p>
            <p>且此电子邮件已加入至审核白名单, 以后使用本邮箱提交的评论无需审核即可展示.</p>
            <p>当您的评论被回复并通过审核时, 您将会收到相关邮件.</p>
            <p>您的评论内容如下</p>
            <p></p>{(c.Respond != null ? $"<p>@{c.Respond.Name}:{c.Respond.Content}</p>" : "")}
            <p>{c.Content}</p>
            <hr/>
            <p>谢谢您的评论</p>
            <p>站长 {Settings.Global.Hoster.Value}</p>
            """;

            return SendMail(c.Email, c.Name, $"您已被加入评论白名单!", Content);
        }
        public bool SendCommentReplyNotice(Comment c)
        {
            string Content = $"""
                <h2>您在文章<a href="{thishostUrl}/read/{c.PostID}">《{c.PostData.Title}》</a>上的评论收到了回复.</h2>
                <hr/>
                <p>这是您的评论</p>
                <p>{c.Respond.Name}:{c.Respond.Content}</p>
                <p>这是对方的回复</p>
                <p>{c.Name}:{c.Content}</p>
                <hr/>
                <p>若需要回复该评论, <a href="{thishostUrl}/read/{c.PostID}">点击此处进行跳转</a>.</p>
            """;
            return SendMail(c.Respond.Email, c.Respond.Name, $"{c.Respond.Name}, 您的评论有新回复!", Content);
        }
        public bool SendWaitVerifyCommentToManager(Comment c) { 
            string Content = $"""
                <h2>有新评论需要审核</h2>
                <hr/>
                <p>文章: <a href="{thishostUrl}/read/{c.PostID}">{c.PostData.Title}</a></p>
                <p>时间: {c.Time}</p>
                <p>邮箱: {c.Email}</p>
                <p>IP: {c.EndPoint}</p>
                <p>审核链接: <a href="{thishostUrl}/commentmgr">点击此处进行审核</a></p>
            """;
            return SendMail(Settings.SMTP.ManagerEmail.Value, Settings.Global.Hoster.Value, "有新评论需要审核", Content);
        }
    }
}
