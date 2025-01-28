using System.IO;
using System.Net;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DBConvert
{
    public class MySqlDB
    {
        MySqlConnection conn;
        public MySqlDB(IPEndPoint point, string user,string password,string database)
        {
            MySqlConnectionStringBuilder build = new MySqlConnectionStringBuilder();
            build.Server = point.Address.ToString();
            build.Port = (uint)point.Port;
            build.UserID = user;
            build.Password = password;
            build.Database = database;
            conn = new MySqlConnection(build.ConnectionString);
            conn.Open();
        }
        public MySqlDataReader Query(string cmd, params ValueTuple<string, object>[] args)
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

        public string? GetConfig(string key,string Namespace = "global")
        {
            return (string?)QueryValue("SELECT value FROM settings WHERE `namespace`=@nspace AND `key`=@key",
                ("@nspace", Namespace),
                ("@key", key));
        }
        public string GetConfig(string key, string Namespace = "global", string nullret = "")
        {
            string? val = GetConfig(key, Namespace);
            if (val == null)
            {
                return nullret;
            }
            else return val;
        }
        public void SetConfig(string key, string? value = null, string Namespace = "global")
        {
            this.Execute("INSERT OR REPLACE INTO settings(`namespace`,`key`,`value`) VALUES(@nspace,@key,@val)",
                ("@nspace", Namespace),
                ("@key", key),
                ("@val", value != null ? value : "NULL"));
        }
        public int StructVersion
        {
            get
            {
                try
                {
                    var get = GetConfig("__ROOT__", "VERSION");
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
