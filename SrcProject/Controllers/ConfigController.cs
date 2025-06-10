using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SrcProject.Controllers
{
    [Route("Config")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("ChangeLogo")]
        public async Task<ActionResult> PostChangeLogo(IFormFile file)
        {
            try
            {
                var response = await Utilities.Utils.ChangeImage(file, _configuration["FilePaths:Logo"]);

                return StatusCode(StatusCodes.Status200OK,
                      new { IsSuccess = true, Message = response.IsSuccess == true ? "Imagen actualizada con éxito" : "No se pudo actualizar la imagen", Result = response });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = false, Message = "Error interno del servidor.", Result = ex.Message });
            }
        }

    }
}
