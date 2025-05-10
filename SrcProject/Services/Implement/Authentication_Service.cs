using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using SrcProject.Models.InModels;
using SrcProject.Models.OutModels;
using SrcProject.Services.Contract;
using SrcProject.Utilities;
using System.Data;

namespace SrcProject.Services.Implement
{
    public class Authentication_Service : IAuthentication_Service
    {
        private readonly IConfiguration _configuration;
        private readonly Jwt _jwt;

        public Authentication_Service(IConfiguration configuration, Jwt jwt)
        {
            _configuration = configuration;
            _jwt = jwt;
        }

        public async Task<ResponseManager> Login(LoginIM loginIM)
        {
            try
            {
                var externalUserLogin = await GetExternalUserLogin(loginIM.strUserName, loginIM.strPassword);

                if (externalUserLogin.strDni != null)
                {
                    string strUserType = "ext";
                    return await _jwt.BuildToken(strUserType, externalUserLogin.strName, externalUserLogin.strLastName, externalUserLogin.strEmail);
                }

                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "El nombre de usuario o la contraseña no son correctos."
                };
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método LoginAsync" + ex.Message);
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        private async Task<LoginOM> GetExternalUserLogin(string userName, string password)
        {
            try
            {
                var user = new LoginOM();

                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnnDbDeMisManos"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_GetExternalUserLogin", cnn);
                    cmd.Parameters.AddWithValue("pEmail", userName);
                    cmd.Parameters.AddWithValue("pDni", password);

                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            user.strDni = dr["strDni"].ToString();
                            user.strName = dr["strName"].ToString();
                            user.strLastName = dr["strLastName"].ToString();
                            user.strEmail = dr["strEmail"].ToString();
                        }
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método GetExternalUserLogin " + ex.Message);
                throw;
            }
        }

        public async Task<List<PermissionsOM>> GetPermissionsByUser(string strUserName)
        {
            try
            {
                List<PermissionsOM> permisions = new List<PermissionsOM>();

                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnnDbDeMisManos"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_GetPermissionsByUser", cnn);
                    cmd.Parameters.AddWithValue("pUsuario", strUserName);

                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            string strModule = dr["strModule"].ToString();
                            string strSubModule = dr["strSubModule"].ToString();

                            var existingPermission = permisions.FirstOrDefault(p => p.strModule == strModule);

                            if (existingPermission != null)
                            {
                                if (!existingPermission.strSubModules.Any(s => s.strSubModule == strSubModule))
                                {
                                    existingPermission.strSubModules.Add(new RoutesOM { strSubModule = strSubModule });
                                }
                            }
                            else
                            {
                                var newPermission = new PermissionsOM
                                {
                                    strModule = strModule,
                                    strSubModules = new List<RoutesOM> { new RoutesOM { strSubModule = strSubModule } }
                                };
                                permisions.Add(newPermission);
                            }
                        }
                    }
                }
                return permisions;
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método GetPermissionsByUserAsync " + ex.Message);
                throw;
            }
        }

    }
}