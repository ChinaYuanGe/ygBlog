using System.IO;
using Microsoft.Data.Sqlite;

namespace ygBlog.Database
{
    public class Database
    {
        SqliteConnection conn;
        public Database(string path)
        {
            SqliteConnectionStringBuilder build = new SqliteConnectionStringBuilder();
            build.DataSource = path;
            build.Mode = SqliteOpenMode.ReadWriteCreate;
            conn = new SqliteConnection(build.ConnectionString);
            if (conn.State != System.Data.ConnectionState.Open) {
                conn.Open();
            }
            try
            {
                if (StructVersion <= 0) {
                    Upgrade.DoUpgrade(this);
                }
            }
            catch (Exception ex)
            {
                Upgrade.DoUpgrade(this);
            }
        }
        public SqliteDataReader Query(string cmd, params ValueTuple<string, object>[] args)
        {
            var sqlcmd = conn.CreateCommand();
            sqlcmd.CommandText = cmd;
            foreach (var arg in args)
            {
                sqlcmd.Parameters.AddWithValue(arg.Item1, arg.Item2);
            }
            return sqlcmd.ExecuteReader();

        }
        public int Execute(string cmd, params ValueTuple<string, object>[] args)
        {
            var sqlcmd = conn.CreateCommand();
            sqlcmd.CommandText = cmd;
            foreach (var arg in args)
            {
                sqlcmd.Parameters.AddWithValue(arg.Item1, arg.Item2);
            }
            return sqlcmd.ExecuteNonQuery();
        }
        public object? QueryValue(string cmd, params ValueTuple<string, object>[] args)
        {
            var sqlcmd = conn.CreateCommand();
            sqlcmd.CommandText = cmd;
            foreach (var arg in args)
            {
                sqlcmd.Parameters.AddWithValue(arg.Item1, arg.Item2);
            }
            return sqlcmd.ExecuteScalar();
        }
        public void Close()
        {
            conn.Close();
        }

        public string? GetConfig(string key, string? nullret = null, string Namespace = "g")
        {
            var res = QueryValue("SELECT value FROM settings WHERE `namespace`=@nspace AND `key`=@key",
                ("@nspace", Namespace),
                ("@key", key));
            return (res != null && res.GetType() != typeof(DBNull)) ? res.ToString() : nullret;
        }
        public int SetConfig(string key, string? value = null, string Namespace = "g")
        {
            if (GetConfig(key, null, Namespace) != null)
            {
                return this.Execute("UPDATE settings SET `value`=@val WHERE `namespace`=@nspace AND `key`=@key",
                    ("@nspace", Namespace),
                    ("@key", key),
                    ("@val", value != null ? value : DBNull.Value));
            }
            else
            {
                return this.Execute("INSERT INTO settings(`namespace`,`key`,`value`) VALUES(@nspace,@key,@val)",
                ("@nspace", Namespace),
                ("@key", key),
                ("@val", value != null ? value : DBNull.Value));
            }
        }
        public int StructVersion
        {
            get
            {
                try
                {
                    var get = GetConfig("VERSION", null, "__ROOT__");
                    if (get != null)
                    {
                        return int.Parse(get);
                    }
                    else return -1;
                }
                catch { return -1; }
            }
        }
        public long LastInsertRowID => (long)QueryValue("SELECT LAST_INSERT_ROWID()");
    }
}
