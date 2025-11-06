using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.Domain.DTO;
using Users.Domain.Services.Interface;
using static Users.Domain.DTO.ErrorDto;

namespace Users.API.Controllers;

[Route("api/v1")]
[ApiController]
public class UserController([FromServices] IPerfilServices perfilServices, IUserServices userServices, ILoggerServices logger) : ControllerBase
{
    [HttpPost]
    [Route("criar/perfil")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CriarPerfil(
    [FromBody] PerfilDto.CriarPerfilRequest request) {
        await logger.LogInformation("Iniciou CriarPerfil");
        var result = await perfilServices.CriarPerfil(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    [Route("entrar/usuario")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> EntrarNoSistema(
    [FromBody] LoginDto.LoginDtoRequest request) {
        await logger.LogInformation("Iniciou EntrarNoSistema");
        var result = await userServices.Entrar(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
