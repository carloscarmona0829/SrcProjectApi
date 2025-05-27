using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SrcProject.Models.InModels;
using SrcProject.Models.InModels.Security;
using SrcProject.Services.Contract.Security;
using SrcProject.Utilities;

namespace SrcProject.Controllers.Security
{
    [Route("Auth")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentication_Service _authService;
        private readonly IConfiguration _configuration;
        public AuthenticationController(IAuthentication_Service authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModelIM registerModelIM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _authService.Register(registerModelIM);

                    if (result.IsSuccess)
                    {
                        return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            issuccess = result.IsSuccess,
                            message = result.Message,
                        });
                    }

                    return StatusCode(StatusCodes.Status200OK, new { IsSuccess = false, Result = result.Message });
                }

                return BadRequest("Algunas propiedades no son válidas.");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
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
                        Jwt jwtGenerator = new Jwt(_configuration);
                        var token = await jwtGenerator.BuildToken(result.Data.UserType, result.Data.FirstName, result.Data.LastName, result.Data.Email);
                        var responsePermissions = await _authService.GetPermissionsByUser(loginIM);

                        return StatusCode(StatusCodes.Status200OK,
                      new
                      {
                          issuccess = result.IsSuccess,
                          message = result.Message,
                          token = token.Data.Token,
                          expireDate = token.Data.ExpireDate,
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
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }

        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                    return NotFound();

                var result = await _authService.ConfirmEmail(userId, token);

                if (result.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status200OK,
                         new
                         {
                             issuccess = result.IsSuccess,
                             message = result.Message,
                         });
                }

                return StatusCode(StatusCodes.Status200OK, new { IsSuccess = false, Result = result.Message });               
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }
    }
}
