using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public static class DB
    {
       
        public static string ConnectionString(IConfiguration _config)
        {
            string _encrypt = "!dcrpos!";
            var server = Encryption.Decrypt(_config["DB:server"], _encrypt);
            var database = Encryption.Decrypt(_config["DB:database"], _encrypt);
            var username = Encryption.Decrypt(_config["DB:username"], _encrypt);
            var password = Encryption.Decrypt(_config["DB:password"], _encrypt);

            string connect = $"Server={server}; Initial Catalog={database}; User id={username}; Password={password}";
            return connect;
        }

        public static void Log(string msg, string controller, string method, IConfiguration config)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DB.ConnectionString(config)))
                {
                    conn.Open();
                    string sql = $"insert into web_log_tab (message, controller, method) values ('{msg}','{controller}','{method}')";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
    }
}
