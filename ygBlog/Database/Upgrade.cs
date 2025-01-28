namespace ygBlog.Database
{
    public class Upgrade
    {
        // Database version always bigger than Dic's biggest int
        static Dictionary<int, Action<Database>> UpgradeDic = new Dictionary<int, Action<Database>> {
            { 0,(d)=>{ // The first init.
                d.Execute("DROP TABLE IF EXISTS `settings`");
                d.Execute(@"CREATE TABLE settings (
                    namespace TEXT NOT NULL,
                    key       TEXT NOT NULL,
                    value     TEXT
                )");
                d.Execute("DROP TABLE IF EXISTS `groups`");
                d.Execute(@"CREATE TABLE groups (
                    id   INTEGER NOT NULL
                        PRIMARY KEY AUTOINCREMENT,
                    name TEXT    NOT NULL
                );");
                d.Execute("DROP TABLE IF EXISTS `posts`");
                d.Execute(@"CREATE TABLE posts (
                    id              INTEGER NOT NULL 
                                    PRIMARY KEY AUTOINCREMENT,
                    title           TEXT    DEFAULT NULL,
                    [group]         INTEGER NOT NULL
                                    DEFAULT '0',
                    tags            TEXT    DEFAULT NULL,
                    time_pubish     INTEGER NOT NULL
                                    DEFAULT (0),
                    time_lastedit   INTEGER NOT NULL
                                    DEFAULT (0),
                    status          INTEGER NOT NULL
                                    DEFAULT '0',
                    counter_visit   INTEGER NOT NULL
                                    DEFAULT '0',
                    counter_comments INTEGER NOT NULL
                                    DEFAULT '0'
                    )");
                d.Execute("DROP TABLE IF EXISTS `comments`");
                d.Execute(@"CREATE TABLE comments (
                    id      INTEGER NOT NULL
                            PRIMARY KEY AUTOINCREMENT,
                    name    TEXT    NOT NULL,
                    email   TEXT    NOT NULL,
                    content TEXT    NOT NULL,
                    artid   INTEGER NOT NULL,
                    resp    INTEGER NOT NULL
                            DEFAULT '0',
                    visible INTEGER NOT NULL
                            DEFAULT '0',
                    time    INTEGER NOT NULL,
                    endpoint TEXT    NOT NULL
                );");
                d.Execute("DROP TABLE IF EXISTS `comment_whitelist`");
                d.Execute(@"CREATE TABLE `comment_whitelist` (
                    `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    `email` TEXT NOT NULL)");
                
            } }
        };
        public static void DoUpgrade(Database db)
        {
            int dbVer = db.StructVersion;
            for (; dbVer <= UpgradeDic.Keys.Max(); dbVer++) {
                if (UpgradeDic.ContainsKey(dbVer)) {
                    UpgradeDic[dbVer].Invoke(db);
                }
            }
            db.SetConfig("VERSION", dbVer.ToString(), "__ROOT__");
        }
    }
}
