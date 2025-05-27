using System.Net;
using System.Net.Mail;

namespace SrcProject.Utilities
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
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
                    LogManager.DebugLog($"Email enviado exitosamente a {toEmail}.");                   
                }
                catch (SmtpException ex)
                {
                    LogManager.DebugLog($"Error SMTP al enviar email a {toEmail}: {ex.Message} + \"Detalles del error: {{ex.StatusCode}}, {{ex.InnerException?.Message}}\"");                                    
                }
                catch (Exception ex)
                {
                    LogManager.DebugLog($"Error inesperado al enviar email a {toEmail}: {ex.Message} + \"Detalles del error: {{ex.StatusCode}}, {{ex.InnerException?.Message}}\"");                    
                }
            }
        }
    }
}
