using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SrcProject.Utilities
{
    public class Jwt
    {
        private readonly IConfiguration _configuration;

        public Jwt(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ResponseManager> BuildToken(string strUserType, string strName, string strLastName, string strEmail)
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

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:JWTkey"] ?? string.Empty));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWTSettings:Issuer"],
                    audience: _configuration["JWTSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JWTSettings:ExpiresMinutes"])),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

                string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                return new ResponseManager
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
                LogManager.DebugLog("Error en la creación del token. " + ex.Message);
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "Error en la creación del token. " + ex.Message
                };
            }
        }
    }
}
