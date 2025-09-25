using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SrcProject.Models.InModels.Security;
using SrcProject.Utilities;

namespace SrcProject.Controllers
{
    [Route("InOut")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InOutController : ControllerBase
    {
        private readonly Utils _utils; // Declara una variable de solo lectura

        // Inyecta Utils en el constructor
        public InOutController(Utils utils)
        {
            _utils = utils;
        }

        [HttpPost("Card")]
        [AllowAnonymous]
        public async Task<ActionResult> Card([FromBody] CardIM cardIM)
        {
            try
            {
                var response = await _utils.GetGenerateQr(cardIM.strUserId, cardIM.strUserName);

                return StatusCode(StatusCodes.Status200OK,response);                      
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }
    }
}
