using Microsoft.IdentityModel.Tokens;
using QRCoder;
using System.IdentityModel.Tokens.Jwt;
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

        public static async Task<string> GetBase64Image(string route, string name)
        {
            /*Método para mostrar imagenes en general, recibe la ruta y el nombre de la imagen
             Este método no tiene api sino que se usa como solicitud en NewsSercive.cs */

            string imagePath = Path.Combine(route, name);

            if (!File.Exists(imagePath))
            {
                return imagePath ?? string.Empty;
            }

            imagePath = imagePath ?? string.Empty;
            var imageBytes = File.ReadAllBytes(imagePath);
            var base64ImageString = Convert.ToBase64String(imageBytes);

            return base64ImageString;
        }
        public static async Task<ResponseManager> ChangeImage(string fileName, string filePath, IFormFile file)
        {
            if (fileName == null || string.IsNullOrEmpty(fileName))
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "Nombre de la imagen no válido. ",
                    Response = null
                };
            }
            if (filePath == null || string.IsNullOrEmpty(filePath))
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "Ruta de archivo no válida. ",
                    Response = null
                };
            }
            if (file == null || file.Length == 0)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "Archivo no válido. ",
                    Response = null
                };
            }

            var currentFilePath = Path.Combine(filePath, fileName);
            if (System.IO.File.Exists(currentFilePath))
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var oldFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var oldFileExtension = Path.GetExtension(fileName);

                var newOldFileName = $"{oldFileNameWithoutExtension}_{timestamp}{oldFileExtension}";
                var newOldFilePath = Path.Combine(filePath, newOldFileName);

                try
                {
                    System.IO.File.Move(currentFilePath, newOldFilePath);
                    LogManager.DebugLog($"Antigua imagen renombrada a: {newOldFileName}");
                }
                catch (Exception ex)
                {
                    LogManager.DebugLog("Error en el método UploadFile. " + ex.Message);
                    return new ResponseManager
                    {
                        IsSuccess = false,
                        Message = "Error en el método UploadFile.",
                        Response = ex.Message
                    };
                }
            }

            using (var stream = new FileStream(currentFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return new ResponseManager
            {
                IsSuccess = true,
                Message = "Imagen actualizada exitosamente.",
                Response = null
            };
        }
        public async Task<ResponseManager> GetGenerateQr(string strUserId, string strUserName)
        {
             try
            {
                var claims = new List<Claim>()
            {
                  new Claim("strUserId", strUserId),
                  new Claim("strUserName", strUserName),                  
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:JWTkey"] ?? string.Empty));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWTSettings:Issuer"],
                    audience: _configuration["JWTSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JWTSettings:ExpiresMinutesCard"])),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

                string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                // 3. Generar la imagen del código QR a partir del token
                var qrGenerator = new QRCodeGenerator();
                var qrData = qrGenerator.CreateQrCode(tokenAsString, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrData);

                // 4. Convertir la imagen a un array de bytes para enviarla como respuesta
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                return new ResponseManager
                {
                    IsSuccess = true,
                    Message = "El código QR se generó exitosamente.",
                    Response = qrCodeImage
                };
            }
            catch (Exception ex)
            {
                LogManager.DebugLog("Error en la creación del código QR. " + ex.Message);
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "Error en la creación del  QR. " + ex.Message
                };
            }
           
        }
    }
}
