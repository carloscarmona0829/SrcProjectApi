using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels.Security;
using SrcProject.Services.Contract.Security;
using SrcProject.Utilities;
using System.Data;
using System.Security;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SrcProject.Services.Implement.Security
{
    public class Authorization_Service : IAuthorization_Service
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public Authorization_Service(IConfiguration configuration)
        {
            _configuration = configuration;
        }              

        public async Task<List<PermissionsByUserByRouteOM>> GetPermissionsByUserByRoute(PermissionsIM permissionsIM)
        {
            List<PermissionsByUserByRouteOM> lst = new List<PermissionsByUserByRouteOM>();

            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_GetPermissionsByUserByRoute", cnn);
                    cmd.Parameters.AddWithValue("pUserName", permissionsIM.UserName);
                    cmd.Parameters.AddWithValue("pRouteName", permissionsIM.RouteName);

                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lst.Add(new PermissionsByUserByRouteOM()
                            {
                                IdPermission = Convert.ToInt32(dr["IdPermission"]),
                                FullName = dr["FullName"].ToString() ?? string.Empty,
                                UserName = dr["UserName"].ToString() ?? string.Empty,
                                Permission = dr["Permission"].ToString() ?? string.Empty
                            });
                        }
                    }
                    return lst;
                }
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método GetPermissionsByUser " + ex.Message);
                throw;
            }
        }

        public async Task<ResponseManager> AddPermissionsByUser(PermissionsIM permissionsIM)
        {
            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_AddPermissionsByUser", cnn);
                    cmd.Parameters.AddWithValue("pIdRoute", permissionsIM.IdPermission);
                    cmd.Parameters.AddWithValue("pUserName", permissionsIM.UserName);

                    cmd.CommandType = CommandType.StoredProcedure;

                    var msj = string.Empty;
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        if (dr.Read())
                        {
                            dr.GetString(dr.GetOrdinal("msj"));
                            msj = dr.GetString(dr.GetOrdinal("msj"));
                        }
                        return new ResponseManager
                        {
                            IsSuccess = true,
                            Message = msj,
                            Response = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método AddPermissionsByUser " + ex.Message);
                throw;
            }
        }

        public async Task<ResponseManager> DeletePermissionsByUser(PermissionsIM permissionsIM)
        {
            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_DeletePermissionsByUser", cnn);
                    cmd.Parameters.AddWithValue("pIdPermission", permissionsIM.IdPermission);
                    cmd.Parameters.AddWithValue("pUserName", permissionsIM.UserName);

                    cmd.CommandType = CommandType.StoredProcedure;

                    var msj = string.Empty;
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        if (dr.Read())
                        {
                            dr.GetString(dr.GetOrdinal("msj"));
                            msj = dr.GetString(dr.GetOrdinal("msj"));
                        }
                        return new ResponseManager
                        {
                            IsSuccess = true,
                            Message = msj,
                            Response = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método DeletePermissionsByUser " + ex.Message);
                throw;
            }
        }

        public async Task<List<object>> GetRoutes()
        {
            List<object> lst = new List<object>();

            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    string sqlQuery = "SELECT IdRoute, RouteName FROM tbl_Pwa_Sec_Routes WHERE Active = 1";
                    SqlCommand cmd = new SqlCommand(sqlQuery, cnn);

                    cmd.CommandType = CommandType.Text;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lst.Add(new
                            {
                                intRouteId = Convert.ToInt32(dr["IdRoute"]),
                                strRouteName = dr["RouteName"].ToString()
                            });
                        }
                    }
                    return lst;
                }
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método GetRoutes " + ex.Message);
                throw;
            }
        }

        public async Task<List<object>> GetUser()
        {
            List<object> lst = new List<object>();

            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    string sqlQuery = "SELECT Dni, FirstName + ' ' + LastName AS FullName, UserName FROM AspNetUsers WHERE EmailConfirmed = 1 ORDER BY FirstName ASC";
                    SqlCommand cmd = new SqlCommand(sqlQuery, cnn);

                    cmd.CommandType = CommandType.Text;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lst.Add(new
                            {
                                strDni = dr["Dni"].ToString(),
                                strUserName = dr["UserName"].ToString(),
                                strFullName = dr["FullName"].ToString()
                            });
                        }
                    }
                    return lst;
                }
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método GetUser " + ex.Message);
                throw;
            }
        }
    }
}

