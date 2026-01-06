using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.Domain.DTO;
using Users.Domain.Services.Interface;
using static Users.Domain.DTO.ErrorDto;

namespace Users.API.Controllers;

[Route("api/v1")]
[ApiController]
public class WalletController(
    ICarteiraServices services, 
    ILoggerServices logger,
    [FromServices] IMetricsService metricsService) : ControllerBase
{
    /// <summary>
    /// Adiciona ou atualiza o saldo de um usuário na carteira.
    /// </summary>
    /// <param name="request">Dados do saldo a ser inserido ou atualizado (contendo o ID do usuário e o valor do saldo).</param>
    /// <returns>Retorna o saldo atualizado ou erro de validação.</returns>
    /// <response code="200">Saldo inserido/atualizado com sucesso.</response>
    /// <response code="401">Não autorizado.</response>
    /// <response code="403">Acesso proibido.</response>
    [HttpPost]
    [Route("adicionar/saldos")]
    [ProducesResponseType(typeof(CarteiraDto.CarteiraDtoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> InserirSaldos(
        [FromBody] CarteiraDto.CarteiraDtoRequest request) {
        
        if (request == null || request.PerfilId == Guid.Empty)
        {
            return BadRequest(new ErrorResponse { Message = "Request inválido ou PerfilId obrigatório", Code = "400" });
        }
        
        if (request.Saldo <= 0)
        {
            return BadRequest(new ErrorResponse { Message = "Saldo deve ser maior que zero", Code = "400" });
        }
        
        await logger.LogInformation("Iniciou InserirSaldos");
        
        var result = await services.InsereSaldos(request);
        
        if (result.IsSuccess)
        {
            // Verificar se é criação ou atualização
            var existingWallet = await services.ConsultaSaldos(request.PerfilId);
            if (!existingWallet.IsSuccess)
            {
                metricsService.IncrementWalletsCreated();
            }
            else
            {
                metricsService.IncrementWalletsUpdated();
            }
            
            return Ok(result.Value);
        }
        
        return BadRequest(result.Error);
    }

    /// <summary>
    /// Consulta o saldo da carteira de um usuário específico.
    /// </summary>
    /// <param name="UsuarioId">ID do usuário para consulta do saldo.</param>
    /// <returns>Retorna o saldo encontrado ou erro de validação.</returns>
    /// <response code="200">Consulta realizada com sucesso.</response>
    /// <response code="401">Não autorizado.</response>
    /// <response code="403">Acesso proibido.</response>
    [HttpGet]
    [Route("consulta/saldos")]
    [ProducesResponseType(typeof(CarteiraDto.CarteiraDtoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConsultaSaldos(
        [FromHeader] Guid UsuarioId) {
        
        if (UsuarioId == Guid.Empty)
        {
            return BadRequest(new ErrorResponse { Message = "UsuarioId é obrigatório", Code = "400" });
        }
        
        await logger.LogInformation("Iniciou ConsultaSaldos");
        metricsService.IncrementWalletsQueried();
        
        var result = await services.ConsultaSaldos(UsuarioId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete]
    [Route("remover/saldos")]
    [ProducesResponseType(typeof(CarteiraDto.CarteiraDtoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoverSaldos(
    [FromBody] CarteiraDto.CarteiraDtoRequest request) {
        
        if (request == null || request.PerfilId == Guid.Empty)
        {
            return BadRequest(new ErrorResponse { Message = "Request inválido ou PerfilId obrigatório", Code = "400" });
        }
        
        if (request.Saldo <= 0)
        {
            return BadRequest(new ErrorResponse { Message = "Valor a remover deve ser maior que zero", Code = "400" });
        }
        
        await logger.LogInformation("Iniciou RemoverSaldos");
        
        var result = await services.RemoverSaldos(request);
        
        if (result.IsSuccess)
        {
            metricsService.IncrementWalletsUpdated();
            return Ok(result.Value);
        }
        
        return BadRequest(result.Error);
    }
}
