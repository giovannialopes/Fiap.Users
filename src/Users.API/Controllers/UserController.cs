using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.Domain.DTO;
using Users.Domain.Services.Interface;
using static Users.Domain.DTO.ErrorDto;

namespace Users.API.Controllers;

/// <summary>
/// Controller para operações de usuários
/// </summary>
[Route("api/v1")]
[ApiController]
public class UserController(
    [FromServices] IPerfilServices perfilServices, 
    IUserServices userServices, 
    ILoggerServices logger,
    [FromServices] IMetricsService metricsService) : ControllerBase
{
    /// <summary>
    /// Cria um novo perfil de usuário
    /// </summary>
    /// <param name="request">Dados do perfil a ser criado</param>
    /// <returns>Dados do perfil criado com token JWT</returns>
    /// <response code="200">Perfil criado com sucesso</response>
    /// <response code="400">Erro na validação</response>
    [HttpPost]
    [Route("criar/perfil")]
    [ProducesResponseType(typeof(PerfilDto.PerfilResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarPerfil(
    [FromBody] PerfilDto.CriarPerfilRequest request) {
        
        if (request == null)
        {
            return BadRequest(new ErrorResponse { Message = "Request não pode ser nulo", Code = "400" });
        }
        
        await logger.LogInformation("Iniciou CriarPerfil");
        var result = await perfilServices.CriarPerfil(request);
        
        if (result.IsSuccess)
        {
            metricsService.IncrementUsersCreated();
            return Ok(result.Value);
        }
        
        return BadRequest(result.Error);
    }

    /// <summary>
    /// Autentica um usuário no sistema
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT e dados do usuário</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Credenciais inválidas</response>
    [HttpPost]
    [Route("entrar/usuario")]
    [ProducesResponseType(typeof(LoginDto.LoginDtoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EntrarNoSistema(
    [FromBody] LoginDto.LoginDtoRequest request) {
        
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
        {
            return BadRequest(new ErrorResponse { Message = "Email e senha são obrigatórios", Code = "400" });
        }
        
        await logger.LogInformation("Iniciou EntrarNoSistema");
        var result = await userServices.Entrar(request);
        
        if (result.IsSuccess)
        {
            metricsService.IncrementUsersLoggedIn();
            return Ok(result.Value);
        }
        
        return BadRequest(result.Error);
    }
}
