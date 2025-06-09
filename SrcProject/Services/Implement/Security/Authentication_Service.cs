using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels;
using SrcProject.Models.OutModels.Security;
using SrcProject.Services.Contract.Security;
using SrcProject.Utilities;
using System.Data;
using System.Text;

namespace SrcProject.Services.Implement.Security
{
    public class Authentication_Service : IAuthentication_Service
    {
        private readonly UserManager<ApplicationUserIM> _userManager;
        private readonly SignInManager<ApplicationUserIM> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly Utils _utils;

        public Authentication_Service(UserManager<ApplicationUserIM> userManager, SignInManager<ApplicationUserIM> signInManager,IConfiguration configuration, Utils utils)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _utils = utils;
        }

        public async Task<ResponseManagerOM> Register(RegisterModelIM registerModelIM)
        {
            try
            {
                if (registerModelIM == null)
                    throw new NullReferenceException("Register Model is Null");

                if (registerModelIM.Password != registerModelIM.ConfirmPassword)
                {
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = "Las contraseñas no coinciden.",
                        Response = null
                    };
                }

                var user = new ApplicationUserIM
                {
                    Dni = registerModelIM.Dni,
                    FirstName = registerModelIM.FirstName,
                    LastName = registerModelIM.LastName,
                    BirthDay = registerModelIM.BirthDay,
                    UserName = registerModelIM.Email.Substring(0, registerModelIM.Email.IndexOf('@')),
                    Email = registerModelIM.Email,
                    PhoneNumber = registerModelIM.PhoneNumber,
                };
               
                var emailExist = await _userManager.FindByEmailAsync(user.Email);

                if (emailExist != null)
                {
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = "El usuario ya existe en la base de datos del sistema.",
                        Response = null
                    };
                }

                var result = await _userManager.CreateAsync(user, registerModelIM.Password);

                if (result.Succeeded)
                {
                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                    var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                    string url = $"{_configuration["Frontend_Local_Url"]}/confirm-email?userid={user.Id}&token={validEmailToken}";

                    await _utils.SendEmail(user.Email, "Confirmar correo electrónico", $"<h2>Sistema de información</h2>" +
                        $"<p>Para confirmar su correo electrónico <a href='{url}'>clic aquí</a></p>");

                    return new ResponseManagerOM
                    {
                        IsSuccess = true,
                        Message = "El usuario fue creado exitosamente.",
                        Response = user
                    };
                }

                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "El usuario no fue creado. ",
                    Response = result.Errors.ElementAtOrDefault(0).Code
                };
            }
            catch (Exception ex)
            {
                Utils.LogManager("Error en el método Register. " + ex.Message);
                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "Error en el método Register. ",
                    Response = ex.Message
                };
            }
        }

        public async Task<ResponseManagerOM> Login(LoginIM loginIM)
        {
            try
            {
                var responseLoginIdentity = await LoginIdentity(loginIM);

                if (responseLoginIdentity.IsSuccess)
                {
                    return new ResponseManagerOM
                    {
                        IsSuccess = true,
                        Message = "Inicio de sesión Exitoso",
                        Response = responseLoginIdentity.Response,
                    };
                }
                else
                {
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = responseLoginIdentity.Message,
                        Response = null
                    };
                }
            }
            catch (Exception ex)
            {
                Utils.LogManager("Error en el método Login. " + ex.Message);
                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "Error en el método Login. ", 
                    Response = ex.Message
                };
            }
        }

        private async Task<ResponseManagerOM> LoginIdentity(LoginIM loginIM)
        {
            try
            {
                var user = loginIM.strUserName.Contains('@') 
                    ? await _userManager.FindByEmailAsync(loginIM.strUserName) 
                    : await _userManager.FindByNameAsync(loginIM.strUserName);                

                if (user != null)
                {
                    string strName = user.FirstName;
                    string strLastName = user.LastName;
                    string strEmail = user.Email;

                    var confirmedEmail = await _userManager.IsEmailConfirmedAsync(user);

                    if (!confirmedEmail)
                        return new ResponseManagerOM
                        {
                            IsSuccess = false,
                            Message = "El usuario no ha confirmado el correo electrónico.",
                            Response = null
                        };

                    var result = await _signInManager.PasswordSignInAsync(user.UserName, loginIM.strPassword,
                       isPersistent: false, lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        return new ResponseManagerOM
                        {
                            IsSuccess = true,
                            Message = "Inicio de sesión exitoso.",
                            Response = new
                            {
                                UserType = "emp", 
                                FirstName = strName,
                                LastName = strLastName,
                                Email = strEmail
                            }
                        };
                    }
                }
                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "Datos de inicio de sesión incorrectos.",
                    Response = null
                };
            }
            catch (Exception ex)
            {
                Utils.LogManager("Error en el método LoginIdentity. " + ex.Message);
                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "Error en el método LoginIdentity. ",
                    Response = ex.Message
                };
            }
        }

        public async Task<ResponseManagerOM> GetPermissionsByUser(LoginIM loginIM)
        {
            try
            {
                List<PermissionsOM> permisions = new List<PermissionsOM>();

                using (var cnn = new SqlConnection(_configuration["ConnectionStrings:cnn"]))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Pwa_Sec_GetPermissionsByUser", cnn);
                    cmd.Parameters.AddWithValue("pUsuario", loginIM.strUserName);

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
                return new ResponseManagerOM
                {
                    IsSuccess = true,
                    Message = "Permisos consultados exitosamente.",
                    Response = permisions
                };
            }
            catch (Exception ex)
            {
                Utils.LogManager("Error en el método GetPermissionsByUser. " + ex.Message);
                throw;
            }
        }

        public async Task<ResponseManagerOM> ConfirmEmail(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = "Confirmación de correo electrónico caducada.",
                        Response = null
                    };

                var decodedToken = WebEncoders.Base64UrlDecode(token);
                string normalToken = Encoding.UTF8.GetString(decodedToken);

                var result = await _userManager.ConfirmEmailAsync(user, normalToken);

                if (!result.Succeeded)
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = "El correo electrónico no fue confirmado.",
                        Response = result.Errors.Select(e => e.Description),
                    };

                return new ResponseManagerOM
                {
                    IsSuccess = true,
                    Message = "Correo electrónico confirmado exitosamente!",
                    Response = null
                };
            }
            catch (Exception ex)
            {
                Utils.LogManager("Error en el método ConfirmEmail. " + ex.Message);
                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "Error en el método LoginIdentity. ",
                    Response = ex.Message
                };
            }
        }

        public async Task<ResponseManagerOM> ForgetPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = "No se encontró ningún usuario asociado a este correo electrónico.",
                        Response = null
                    };

                var confirmedEmail = await _userManager.IsEmailConfirmedAsync(user);

                if (!confirmedEmail)
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = "El correo electrónico no ha sido confirmado.",
                        Response = null
                    };

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodedToken);

                string url = $"{_configuration["Frontend_Local_Url"]}/reset-password?email={email}&token={validToken}";

                await _utils.SendEmail(email, "Restablecer contraseña", $"<h2>Sistema de información</h2>" + 
                    "<h1>Siga las instrucciones para restablecer su contraseña.</h1>" + $"<p>Para restablecer su contraseña <a href='{url}'>clic aquí</a></p>");

                return new ResponseManagerOM
                {
                    IsSuccess = true,
                    Message = "Se ha enviado un mensaje a su correo electrónico con las instrucciones para recuperar su contraseña!",
                    Response = null
                };
            }
            catch (Exception ex)
            {
                Utils.LogManager("Error en el método ForgetPassword. " + ex.Message);
                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "Error en el método ForgetPassword. ",
                    Response = ex.Message
                };
            }
        }

        public async Task<ResponseManagerOM> ResetPassword(ResetPasswordIM resetPasswordIM)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordIM.Email);
                if (user == null)
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = "No se encontró ningún usuario asociado a este correo electrónico.",
                        Response = null
                    };

                if (resetPasswordIM.NewPassword != resetPasswordIM.ConfirmPassword)
                    return new ResponseManagerOM
                    {
                        IsSuccess = false,
                        Message = "Las contraseñas no coinciden.",
                        Response = null
                    };

                var decodedToken = WebEncoders.Base64UrlDecode(resetPasswordIM.Token);
                string normalToken = Encoding.UTF8.GetString(decodedToken);

                var result = await _userManager.ResetPasswordAsync(user, normalToken, resetPasswordIM.NewPassword);

                if (result.Succeeded)
                    return new ResponseManagerOM
                    {
                        IsSuccess = true,
                        Message = "La contraseña ha sido restablecida exitosamente!",
                        Response = null
                    };

                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "No se pudo restablecer la contraseña. ",
                    Response = result.Errors.ElementAtOrDefault(0).Code
                };
            }
            catch (Exception ex)
            {
                Utils.LogManager("Error en el método ResetPassword. " + ex.Message);
                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "Error en el método ResetPassword. ",
                    Response = ex.Message
                };
            }
        }

    }
}