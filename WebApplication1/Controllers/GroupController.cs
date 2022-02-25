using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace WebApplication1.Controllers
{
    [EnableCors("AnyOrigin")]
    [Route("api/groups")]
    public class GroupController : Controller
    {
        private IConfiguration _config { get; set; }
        private IHttpContextAccessor _accessor;

        public GroupController(IConfiguration config, IHttpContextAccessor accessor)
        {
            _config = config;
            _accessor = accessor;
        }

        [HttpPost]
        [Route("create")]
        public ActionResult CreateGroup(string groupname, string apikey)
        {
            try
            {
                var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["user-agent"].ToString();
                if (!Validation.ValidateApiKey(apikey, _config))
                {
                    return Ok(new
                    {
                        error = 99,
                        success = true,
                        msg = "Invalid Api Key"
                    });
                }

                if (groupname.Length == 0)
                {
                    return Ok(new
                    {
                        error = 99,
                        success = true,
                        msg = "Missing Groupname"
                    });
                }

                using (SqlConnection conn = new SqlConnection(DB.ConnectionString(_config)))
                {
                    string sql = "usp_CreateGroup";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@groupname", groupname);
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@useragent", userAgent);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                return Ok(new
                {
                    error = 0,
                    success = true
                });

            }
            catch (Exception ex)
            {
                DB.Log(ex.Message, "group", "create", _config);
                return Ok(new
                {
                    error = 99,
                    success = true,
                    msg = "An internal error occured"
                });
            }
        }

    }
}
