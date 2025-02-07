using System.Net.NetworkInformation;

namespace ygBlog
{
    #region SettingItemModel
    public class Setting {
        Database.Database db;
        public string Key { get; set; }
        public string Namespace { get; set; }
        public string? defaultVal { get; set; }
        public Setting(Database.Database db, string Key, string Namespace = "g",string? def = null)
        {
            this.db = db;
            this.Key = Key;
            this.Namespace = Namespace;
            this.defaultVal = def;
        }
        public string? Value {
            get => Get();
            set => Set(value);
        }
        public string? Get()
        {
            return db.GetConfig(Key, defaultVal, Namespace);
        }
        public void Set(string? value)
        {
            db.SetConfig(Key, value, Namespace);
        }
        public override string ToString()
        {
            return Get();
        }
    }
    #endregion
    public static class Settings
    {
        static Database.Database db;
        public static void SetDB(Database.Database database) { db = database; }
        public static class Global {
            public static Setting Password => new Setting(Settings.db, "password");
            public static Setting Hoster => new Setting(Settings.db, "hoster", def: "g:hoster 未设置");
            public static Setting Hoster_Des => new Setting(Settings.db, "hoster_shortdes", def: "g:hoster_shortdes 未设置");
            public static Setting FooterAppend => new Setting(Settings.db, "footer_append", def: "ygBlog by ChinaYuanGe");
            public static Setting Title => new Setting(Settings.db, "title",def: "ygBlog");
        }
        public static class MainPage
        {
            public static Setting PostOutputLimit => new Setting(Settings.db, "post_output_limit", "mainpage", "5");
            public static Setting MyLinks => new Setting(Settings.db, "my_links", "mainpage", 
                /* href,title,favicon url,bs_button type<\n>next... */
                "mailto:chinayuangel@outlook.com,向我发送邮件,/img/element/email.svg,danger");
            public static Setting RamdonTitle => new Setting(Settings.db, "random_title", "mainpage");
        }
        public static class AvatarService { 
            public static Setting Url => new Setting(Settings.db,"url_template","avatar", "https://cn.gravatar.com/avatar/{0}?s=128&d=retro");
            public static Setting CacheLimit => new Setting(Settings.db, "cache_limit", "avatar", "1000");
            public static Setting CacheLifetime => new Setting(Settings.db, "lifetime", "avatar", "86400");
        }
        public static class Comments {
            public static Setting Enable => new Setting(Settings.db, "enable", "comments", "1");
            public static Setting VerifyEnable => new Setting(Settings.db, "verify_enable", "comments", "1");
            public static Setting VerifyQueueMax => new Setting(Settings.db, "verf_queue_max", "comments", "100");
            public static Setting OutputLimit => new Setting(Settings.db, "output_limit", "comments", "5");
            public static Setting AnonymousName => new Setting(Settings.db, "def_name", "comments", "无名氏");
        }
        public static class SEO
        {
            public static Setting Description => new Setting(Settings.db, "description", "seo", "seo:description 未设置");
            public static Setting Keywords => new Setting(Settings.db, "keywords", "seo", "seo:keywords 未设置");
        }
        public static class SMTP { 
            public static Setting Enable => new Setting(db, "enable", "smtp", "0");
            public static Setting Server => new Setting(db, "server", "smtp", "smtp.example.com");
            public static Setting ServerPort => new Setting(db, "port", "smtp", "25");
            public static Setting Auth_User => new Setting(db, "auth_user", "smtp");
            public static Setting Auth_Pass => new Setting(db, "auth_pass", "smtp");
            public static Setting SenderMail => new Setting(db, "sender", "smtp");
            public static Setting SenderName => new Setting(db, "sendername", "smtp", "ygblog_System");
            public static Setting ManagerEmail => new Setting(db, "manager_email", "smtp", "example@ex.com");
        }
        public static class Other {
            public static Setting BindingDomain => new Setting(db, "binding_domain", "other", "nodomain.com");
        }
    }
}