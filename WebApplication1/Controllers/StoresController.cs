using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.models;

namespace WebApplication1.Controllers
{
    [EnableCors("AnyOrigin")]
    [Route("api/stores")]
    public class StoresController : Controller
    {
        private IConfiguration _config { get; set; }
        private IHttpContextAccessor _accessor;

        public StoresController(IConfiguration config, IHttpContextAccessor accessor)
        {
            _config = config;
            _accessor = accessor;
        }

        [HttpGet]
        [Route("list")]
        public ActionResult GetStoreList(string apikey) 
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
                List<Store> stores = new List<Store>();
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(DB.ConnectionString(_config))) 
                {
                    string sql = "usp_Storelist";
                    using (SqlCommand cmd = new SqlCommand(sql, conn)) 
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                      
                        using (SqlDataAdapter adapt = new SqlDataAdapter(cmd))
                        {
                            adapt.Fill(dt);
                            foreach (DataRow dr in dt.Rows)
                            {
                                Store s = new Store
                                {
                                    Id = dr["id"].ToString(),
                                    StoreNumber = dr["store_number"].ToString(),
                                    StoreName = dr["store_name"].ToString(),
                                    TermCount = dr["term_count"].ToString(),
                                    Version = dr["version"].ToString(),
                                    State = dr["state"].ToString(),
                                    TaxRate = dr["tax_rate"].ToString(),
                                    GroupId = dr["groupid"].ToString()
                                };
                                stores.Add(s);
                            }

                        }
                        conn.Close();
                    }
                    
                }
                return Ok(new
                {
                    error = 0,
                    success = true,
                    stores
                });

            }
            catch (Exception ex) 
            {
                DB.Log(ex.Message, "Stores", "lists", _config);
                return Ok(new
                {
                    error = 2,
                    success = true,
                    msg = "opps, sorry bitches, but things went wrong"
                });
            }
        }


        [HttpPost]
        [Route("create")]
        public ActionResult CreateStore([FromForm] Store store, string apikey) 
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

                if (store.StoreNumber == null)
                {
                    return Ok(new
                    {
                        error = 2,
                        success = true,
                        msg = "Missing Store Number"
                    });
                }

                if (store.StoreNumber.Length == 0)
                {
                    return Ok(new
                    {
                        error = 2,
                        success = true,
                        msg = "Missing Store Number"
                    });
                }

                using (SqlConnection conn = new SqlConnection(DB.ConnectionString(_config))) 
                {
                    string sql = "usp_CreateStore";
                    using (SqlCommand cmd = new SqlCommand(sql, conn)) 
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@storenumber", store.StoreNumber);
                        cmd.Parameters.AddWithValue("@storename", store.StoreName);
                        cmd.Parameters.AddWithValue("@termcount", store.TermCount);
                        cmd.Parameters.AddWithValue("@version", store.Version);
                        cmd.Parameters.AddWithValue("@groupid", store.GroupId);
                        cmd.Parameters.AddWithValue("@state", store.State);
                        cmd.Parameters.AddWithValue("@taxrate", store.TaxRate);
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
                DB.Log(ex.Message, "Stores", "create", _config);
                return Ok(new
                {
                    error = 2,
                    success = true,
                    msg = "opps, sorry bitches, but things went wrong"
                });
            }
        }

        [HttpGet]
        [Route("delete")]
        public ActionResult DeleteStore(string id, string apiKey)
        {
            try
            {            
                var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["user-agent"].ToString();
                if (!Validation.ValidateApiKey(apiKey, _config))
                {
                    return Ok(new
                    {
                        error = 99,
                        success = true,
                        msg = "Invalid Api Key"
                    });
                }

                if (id.Length == 0) 
                {
                    return Ok(new
                    {
                        error = 2,
                        success = true,
                        msg = "Missing STORE ID"
                    });
                };

                using (SqlConnection conn = new SqlConnection(DB.ConnectionString(_config)))
                {
                    string sql = "usp_DeleteStore";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@useragent", userAgent);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                return Ok(new
                {
                    errror = 0,
                    success = true
                });

            }
            catch (Exception ex)
            {
                DB.Log(ex.Message, "stores", "delete", _config);
                return Ok(new
                {
                    error = 99,
                    success = true,
                    msg = "An internal error occured"
                });
            }
        }

        [HttpPost]
        [Route("test")]
        public ActionResult Test()
        {
            return Ok(new
            {
                name = "fred"
            });
        }


        [HttpPut]
        [Route("update")]
        public ActionResult UpdateStore([FromForm] Store store, string apiKey)
        {
            try
            {
                var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["user-agent"].ToString();
                if (!Validation.ValidateApiKey(apiKey, _config))
                {
                    return Ok(new
                    {
                        error = 99,
                        success = true,
                        msg = "Invalid Api Key"
                    });
                }

                if (store.Id == null)
                {
                    return Ok(new
                    {
                        error = 2,
                        success = true,
                        msg = "Missing STORE ID"
                    });
                };

                using (SqlConnection conn = new SqlConnection(DB.ConnectionString(_config)))
                {
                    string sql = "usp_UpdateStore";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", store.Id);
                        cmd.Parameters.AddWithValue("@storenumber", store.StoreNumber);
                        cmd.Parameters.AddWithValue("@storename", store.StoreName);
                        cmd.Parameters.AddWithValue("@termcount", store.TermCount);
                        cmd.Parameters.AddWithValue("@version", store.Version);
                        cmd.Parameters.AddWithValue("@state", store.State);
                        cmd.Parameters.AddWithValue("@groupid", store.GroupId);
                        cmd.Parameters.AddWithValue("@taxrate", store.TaxRate);
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@useragent", userAgent);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                return Ok(new
                {
                    errror = 0,
                    success = true
                });

            }
            catch (Exception ex)
            {
                DB.Log(ex.Message, "stores", "update", _config);
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
