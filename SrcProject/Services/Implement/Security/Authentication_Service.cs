using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using SrcProject.Models.InModels;
using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels;
using SrcProject.Models.Security;
using SrcProject.Services.Contract.Security;
using SrcProject.Utilities;
using System.Data;
using System.Text;

namespace SrcProject.Services.Implement.Security
{
    public class Authentication_Service : IAuthentication_Service
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly Jwt _jwt;

        public Authentication_Service(UserManager<ApplicationUserModel> userManager, SignInManager<ApplicationUserModel> signInManager,IConfiguration configuration, Jwt jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwt = jwt;
        }

        public async Task<ResponseManager> RegisterAsync(RegisterModelIM registerModelIM)
        {
            try
            {
                if (registerModelIM == null)
                    throw new NullReferenceException("Register Model is Null");

                if (registerModelIM.Password != registerModelIM.ConfirmPassword)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Message = "Las contraseñas no coinciden."
                    };
                }

                var user = new ApplicationUserModel
                {
                    Dni = registerModelIM.Dni,
                    FirstName = registerModelIM.FirstName,
                    LastName = registerModelIM.LastName,
                    BirthDay = registerModelIM.BirthDay,
                    UserName = registerModelIM.UserName,
                    Email = registerModelIM.Email,
                    PhoneNumber = registerModelIM.PhoneNumber,
                };

                var result = await _userManager.CreateAsync(user, registerModelIM.Password);

                if (result.Succeeded)
                {
                    //Envío de email de confirmación con token a través de UserManager
                    //var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                    //var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                    //string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={user.Id}&token={validEmailToken}";

                    //SendMail(user.Email, "Confirmar correo electrónico", $"<h2>Sistema de información Country Club Ejecutivos</h2>" +
                    //    $"<p>Para confirmar su correo electrónico <a href='{url}'>clic aquí</a></p>");

                    return new ResponseManager
                    {
                        IsSuccess = true,
                        Message = "El usuario fue creado exitosamente."
                        //Errors = result.Errors.Select(e => e.Description)
                    };
                }

                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "El usuario no fue creado.",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método RegisterAsync");
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResponseManager> Login(LoginIM loginIM)
        {
            try
            {
                var responseLoginIdentity = await LoginIdentity(loginIM);

                if (responseLoginIdentity.IsSuccess)
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Message = responseLoginIdentity.Message,
                    };
                }
                else
                {
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Message = responseLoginIdentity.Message
                    };
                }

                //var externalUserLogin = await GetExternalUserLogin(loginIM.strUserName, loginIM.strPassword);

                //if (externalUserLogin.strDni != null)
                //{
                //    string strUserType = "ext";
                //    return await _jwt.BuildToken(strUserType, externalUserLogin.strName, externalUserLogin.strLastName, externalUserLogin.strEmail);
                //}

                //return new ResponseManager
                //{
                //    IsSuccess = false,
                //    Message = "El nombre de usuario o la contraseña no son correctos."
                //};
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

        private async Task<ResponseManager> LoginIdentity(LoginIM loginIM)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginIM.strEmail);

                if (user != null)
                {
                    var confirmedEmail = await _userManager.IsEmailConfirmedAsync(user);

                    if (!confirmedEmail)
                        return new ResponseManager
                        {
                            Message = "El usuario no ha confirmado el correo electrónico.",
                            IsSuccess = false,
                        };

                    var result = await _signInManager.PasswordSignInAsync(user.UserName, loginIM.strPassword,
                       isPersistent: false, lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        return new ResponseManager
                        {
                            IsSuccess = true,
                            Message = "Inicio de sesión exitoso."
                        };
                    }
                }
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "Datos de inicio de sesión incorrectos."
                };
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en el método LoginIdentity" + ex.Message);
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

                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
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

        public async Task<List<PermissionsOM>> GetPermissionsByUser(LoginIM loginIM)
        {
            try
            {
                List<PermissionsOM> permisions = new List<PermissionsOM>();

                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_GetPermissionsByUser", cnn);
                    cmd.Parameters.AddWithValue("pUsuario", loginIM.strEmail);

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