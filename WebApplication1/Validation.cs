using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public static class Validation
    {
        public static bool ValidateApiKey(string apikey, IConfiguration config) 
        {
            try
            {
                bool success = false;
                using (SqlConnection conn = new SqlConnection(DB.ConnectionString(config)))
                {
                    conn.Open();
                    string sql = "select count(*) from apikeys where apikey='"+ apikey + "'";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    string result = cmd.ExecuteScalar().ToString();
                    int iTmp = 0;
                    int.TryParse(result, out iTmp);
                    if (iTmp > 0)
                    {
                        success = true;
                    }
                    conn.Close();
                }
                return success;
            }
            catch (Exception ex)
            {
                DB.Log(ex.Message, "", "", config);
                return false;
            }
        }
    }
}
