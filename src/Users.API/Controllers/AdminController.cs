using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.Domain.DTO;
using Users.Domain.Services.Interface;
using static Users.Domain.DTO.ErrorDto;

namespace Users.API.Controllers;

[Route("api/v1")]
[ApiController]
public class AdminController([FromServices] IPerfilServices services, ILoggerServices logger) : ControllerBase
{

    [HttpPut]
    [Route("alterar/perfil")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AlterarPerfil(
       [FromBody] PerfilDto.PerfilRequest request,
       [FromQuery] string email,
       [FromQuery] string senha) {
        await logger.LogInformation("Iniciou AlterarPerfil");
        var result = await services.AlterarPerfil(email, senha, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete]
    [Route("deletar/perfil")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeletarPerfil(
      [FromQuery] string email,
      [FromQuery] string senha) {
        await logger.LogInformation("Iniciou DeletarPerfil");
        var result = await services.DeletarPerfil(email, senha);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
