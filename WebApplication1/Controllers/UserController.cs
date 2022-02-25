using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.models;

namespace WebApplication1.Controllers
{
    [EnableCors("AnyOrigin")]
    [Route("api/users")]
    public class UserController : Controller
    {
        private IConfiguration _config { get; set; }
        private IHttpContextAccessor _accessor;

        public UserController(IConfiguration config, IHttpContextAccessor accessor)
        {
            _config = config;
            _accessor = accessor;
        }

        [HttpGet]
        [Route("users")]
        public ActionResult GetUserList(string apikey, string username)
        {
            try
            {
                if (!Validation.ValidateApiKey(apikey, _config))
                {
                    return Ok(new
                    {
                        error = 99,
                        success = true,
                        msg = "Invalid Api Key"
                    });
                }
                List<User> users = new List<User>();
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(DB.ConnectionString(_config)))
                {
                    string sql = "usp_Userlist";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@username", username);


                        using (SqlDataAdapter adapt = new SqlDataAdapter(cmd))
                        {
                            adapt.Fill(dt);
                            foreach (DataRow dr in dt.Rows)
                            {
                                User u = new User
                                {
                                    Id = dr["id"].ToString(),
                                    StoreName = dr["store_name"].ToString(),
                                    State = dr["state"].ToString()
                                };
                                users.Add(u);
                            }

                        }
                        conn.Close();
                    }

                }
                return Ok(new
                {
                    error = 0,
                    success = true,
                    users
                });

            }
            catch (Exception ex)
            {
                DB.Log(ex.Message, "Users", "lists", _config);
                return Ok(new
                {
                    error = 2,
                    success = true,
                    msg = "opps, sorry bitches, but things went wrong"
                });
            }
        }
    }
}
