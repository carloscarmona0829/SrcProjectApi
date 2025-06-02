using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                            Response = result.Response
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
                        var token = await jwtGenerator.BuildToken(result.Response.UserType, result.Response.FirstName, result.Response.LastName, result.Response.Email);
                        var permissions = await _authService.GetPermissionsByUser(loginIM);

                        return StatusCode(StatusCodes.Status200OK,
                      new
                      {
                          issuccess = result.IsSuccess,
                          message = result.Message,
                          token = token.Response.Token,
                          expireDate = token.Response.ExpireDate,
                          permissions = permissions.Response.Count > 0 ? permissions : null,
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
                    new { IsSuccess = false, Message = "Error interno del servidor. ", Result = ex.Message });
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

        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return NotFound();

                var result = await _authService.ForgetPassword(email);

                if (result.IsSuccess)
                    return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                issuccess = result.IsSuccess,
                                message = result.Message,
                            });

                return StatusCode(StatusCodes.Status200OK, new { IsSuccess = false, Result = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }
        
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordIM resetPasswordIM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _authService.ResetPassword(resetPasswordIM);

                    if (result.IsSuccess)
                        return StatusCode(StatusCodes.Status200OK,
                             new
                             {
                                 issuccess = result.IsSuccess,
                                 message = result.Message,
                             });

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
    }
}
