using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels.Security;
using SrcProject.Services.Contract.Security;
using SrcProject.Utilities;
using System.Data;
using System.Security;

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

        public async Task<bool> AddExternalUser(AddExternalUserIM addExternalUserIM)
        {
            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_AddExternalUser", cnn);
                    cmd.Parameters.AddWithValue("pDni", addExternalUserIM.strDni);
                    cmd.Parameters.AddWithValue("pName", addExternalUserIM.strName);
                    cmd.Parameters.AddWithValue("pLastName", addExternalUserIM.strLastName);
                    cmd.Parameters.AddWithValue("pPhone", addExternalUserIM.strPhone);
                    cmd.Parameters.AddWithValue("pEmail", addExternalUserIM.strEmail);
                    cmd.Parameters.AddWithValue("pPartnerId", addExternalUserIM.intPartnerId);

                    cmd.CommandType = CommandType.StoredProcedure;

                    var dr = await cmd.ExecuteReaderAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método AddExternalUser " + ex.Message);
                throw;
            }
        }

        public async Task<List<PermissionsByUserByRouteOM>> GetPermissionsByUser(PermissionsIM permissionsIM)
        {
            List<PermissionsByUserByRouteOM> lst = new List<PermissionsByUserByRouteOM>();

            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_GetPermissionsByUserByRoute", cnn);
                    cmd.Parameters.AddWithValue("pUserName", permissionsIM.strUserName);
                    cmd.Parameters.AddWithValue("pRouteName", permissionsIM.strRoute);

                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lst.Add(new PermissionsByUserByRouteOM()
                            {
                                intPermissionId = Convert.ToInt32(dr["intPermissionId"]),
                                strName = dr["strName"].ToString(),
                                strJobTitle = dr["strJobTitle"].ToString(),
                                strUserName = dr["strUserName"].ToString(),
                                strRoute = dr["strRoute"].ToString()
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

        public async Task<bool> AddPermissionsByUser(PermissionsIM permissionsIM)
        {
            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_AddPermissionsByUser", cnn);
                    cmd.Parameters.AddWithValue("pUserName", permissionsIM.strUserName);
                    cmd.Parameters.AddWithValue("pPermissionId", permissionsIM.intPermissionId);

                    cmd.CommandType = CommandType.StoredProcedure;

                    var dr = await cmd.ExecuteReaderAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método AddPermissionsByUser " + ex.Message);
                throw;
            }
        }

        public async Task<bool> DeletePermissionsByUser(PermissionsIM permissionsIM)
        {
            try
            {
                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_DeletePermissionsByUser", cnn);
                    cmd.Parameters.AddWithValue("pPermissionId", permissionsIM.intPermissionId);
                    cmd.Parameters.AddWithValue("pUserName", permissionsIM.strUserName);

                    cmd.CommandType = CommandType.StoredProcedure;

                    var dr = await cmd.ExecuteReaderAsync();

                    return true;
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
                    string sqlQuery = "SELECT intRouteId, strRouteName FROM tbl_Pwa_Sec_Routes WHERE bitActive = 1";
                    SqlCommand cmd = new SqlCommand(sqlQuery, cnn);

                    cmd.CommandType = CommandType.Text;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lst.Add(new
                            {
                                intRouteId = Convert.ToInt32(dr["intRouteId"]),
                                strRouteName = dr["strRouteName"].ToString()
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
                    string sqlQuery = "SELECT strDni, strName + ' ' + strLastName AS strFullName, strEmail FROM tbl_Pwa_Sec_ExternalUsers WHERE bitState = 1 ORDER BY strName ASC";
                    SqlCommand cmd = new SqlCommand(sqlQuery, cnn);

                    cmd.CommandType = CommandType.Text;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lst.Add(new
                            {
                                strDni = dr["strDni"].ToString(),
                                strUserName = dr["strEmail"].ToString(),
                                strFullName = dr["strFullName"].ToString()
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

