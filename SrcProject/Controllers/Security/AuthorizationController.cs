using SrcProject.Models.OutModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using SrcProject.Services.Contract.Security;
using SrcProject.Models.InModels.Security;

namespace SrcProject.Controllers.Security
{
    [Route("Authorization")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorization_Service _AuthorizationService;

        public AuthorizationController(IAuthorization_Service AutorizationService)
        {
            _AuthorizationService = AutorizationService;
        }                

        [HttpPost("GetPermissionsByUser")]
        public async Task<ActionResult> GetPermissionsByUser([FromBody] PermissionsIM permissionsIM)
        {
            try
            {
                var response = await _AuthorizationService.GetPermissionsByUser(permissionsIM);

                return StatusCode(StatusCodes.Status200OK,
                      new { IsSuccess = true, Message = response?.Count > 0 ? "Consulta realizada con éxito" : "No se obtuvieron resultados.", Result = response });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }

        [HttpPost("AddPermissionsByUser")]
        public async Task<ActionResult> AddPermissionsByUser([FromBody] PermissionsIM permissionsIM)
        {
            try
            {
                var response = await _AuthorizationService.AddPermissionsByUser(permissionsIM);

                return StatusCode(StatusCodes.Status200OK,
                          new { IsSuccess = true, Message = response ? "Permiso concedido con éxito" : "No se pudo realizar la operación.", Result = response });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }

        [HttpPost("DeletePermissionsByUser")]
        public async Task<ActionResult> DeletePermissionsByUser([FromBody] PermissionsIM permissionsIM)
        {
            try
            {
                var response = await _AuthorizationService.DeletePermissionsByUser(permissionsIM);

                return StatusCode(StatusCodes.Status200OK,
                          new { IsSuccess = true, Message = response ? "Permiso revocado con éxito" : "No se pudo realizar la operación.", Result = response });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }

        [HttpGet("GetRoutes")]
        public async Task<ActionResult> GetRoutes()
        {
            try
            {
                var response = await _AuthorizationService.GetRoutes();

                return StatusCode(StatusCodes.Status200OK,
                      new { IsSuccess = true, Message = response?.Count > 0 ? "Consulta realizada con éxito" : "No se obtuvieron resultados.", Result = response });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult> GetUser()
        {
            try
            {
                var response = await _AuthorizationService.GetUser();

                return StatusCode(StatusCodes.Status200OK,
                      new { IsSuccess = true, Message = response?.Count > 0 ? "Consulta realizada con éxito" : "No se obtuvieron resultados.", Result = response });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }
    }
}
