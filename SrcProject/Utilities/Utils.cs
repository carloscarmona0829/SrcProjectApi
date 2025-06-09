using Microsoft.IdentityModel.Tokens;
using SrcProject.Models.OutModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace SrcProject.Utilities
{
    public class Utils
    {
        private readonly IConfiguration _configuration;

        public Utils(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ResponseManagerOM BuildJwt(string strUserType, string strName, string strLastName, string strEmail)
        {
            try
            {
                var claims = new List<Claim>()
            {
                  new Claim("strEmail", strEmail),
                  new Claim("strName", strName),
                  new Claim("strLastName", strLastName),
                  new Claim("strUserType", strUserType)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:JWTkey"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWTSettings:Issuer"],
                    audience: _configuration["JWTSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JWTSettings:ExpiresMinutes"])),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

                string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                return new ResponseManagerOM
                {
                    IsSuccess = true,
                    Message = "El token se generó exitosamente.",
                    Response = new
                    {
                        Token = tokenAsString,
                        ExpireDate = token.ValidTo.ToLocalTime(),
                    }
                };
            }
            catch (Exception ex)
            {
                Utils.LogManager("Error en la creación del token. " + ex.Message);
                return new ResponseManagerOM
                {
                    IsSuccess = false,
                    Message = "Error en la creación del token. ",
                    Response = ex.Message
                };
            }
        }

        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"]);
            var userName = smtpSettings["UserName"];
            var password = smtpSettings["Password"];

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = enableSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(userName, password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(userName, "SrcProject"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                try
                {
                    await client.SendMailAsync(mailMessage);
                    Utils.LogManager($"Email enviado exitosamente a {toEmail}.");
                }
                catch (SmtpException ex)
                {
                    Utils.LogManager($"Error SMTP al enviar email a {toEmail}: {ex.Message} + \"Detalles del error: {{ex.StatusCode}}, {{ex.InnerException?.Message}}\"");
                }
                catch (Exception ex)
                {
                    Utils.LogManager($"Error inesperado al enviar email a {toEmail}: {ex.Message} + \"Detalles del error: {{ex.StatusCode}}, {{ex.InnerException?.Message}}\"");
                }
            }
        }

        public static string LogManager(string mensaje)
        {
            var wrote = "";
            try
            {
                DateTime dt = DateTime.Now;
                string s = dt.ToString("yyyyMMdd");

                // Obtener la ruta del directorio actual del proyecto
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string directoryPath = Path.Combine(baseDirectory, "App-Logs");
                string filePath = Path.Combine(directoryPath, "LogDebug_API_" + s + ".txt");

                // Verificar si la carpeta existe, si no, crearla
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine("{0} >>>> {1}", DateTime.Now, mensaje);
                    sw.Flush();
                }
            }
            catch (Exception u)
            {
                wrote = u.Message.ToString();
            }
            return wrote;
        }
    }
}

