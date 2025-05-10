using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SrcProject.Models.InModels;
using SrcProject.Services.Contract;
using SrcProject.Utilities;

namespace SrcProject.Controllers.Security
{
    [Route("Auth")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentication_Service _authService;
        public AuthenticationController(IAuthentication_Service authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginIM loginIM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _authService.Login(loginIM);

                    if (result.IsSuccess)
                    {
                        //var responseImage = _authService.GetBase64ImageString();
                        var responsePermissions = await _authService.GetPermissionsByUser(loginIM.strUserName);

                        return StatusCode(StatusCodes.Status200OK,
                      new
                      {
                          issuccess = result.IsSuccess,
                          message = result.Message,
                          token = result.Token,
                          expireDate = result.ExpireDate,
                          permissions = responsePermissions.Count > 0 ? responsePermissions : null,
                          //image = responseImage
                      });
                    }
                    return StatusCode(StatusCodes.Status200OK, new { issuccess = false, message = result.Message });
                }

                return BadRequest("Algunas propiedades no son válidas.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }

        [HttpGet("Signin-oidc")]
        [AllowAnonymous]
        public async Task<IActionResult> SignInOidc()
        {
            LogManager.DebugLog("Entró al método signin-oidc");

            // Autentica usando el esquema de OpenID Connect
            var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var userClaims = result.Principal.Claims;

                foreach (var claim in userClaims)
                {
                    LogManager.DebugLog($"Claim type: {claim.Type}, value: {claim.Value}");
                }

                var userName = result.Principal.FindFirst("name")?.Value;
                var userEmail = result.Principal.FindFirst("email")?.Value;

                LogManager.DebugLog($"User authenticated: {userName}, email: {userEmail}");

                return Ok("Authentication successful!");
            }


            // Si no está autenticado, redirigir a Azure AD para el login
            LogManager.DebugLog("No se encontró principal. Redirigiendo a login de Azure AD.");
            return Challenge(new AuthenticationProperties { RedirectUri = "/Auth/Signin-oidc" }, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
