using Users.Domain.DTO;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Users.Domain.Results;
using Users.Domain.Services.Interface;

namespace Users.Domain.Services.Class;

/// <summary>
/// Serviço responsável pelas operações de saldo da carteira dos usuários.
/// </summary>
public class CarteiraServices(ICarteiraRepository carteiraRepository, ILoggerServices logger) : ICarteiraServices
{
    /// <summary>
    /// Consulta o saldo da carteira de um usuário específico.
    /// </summary>
    /// <param name="UsuarioId">Identificador do usuário.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> contendo o saldo da carteira (<see cref="CarteiraDto.CarteiraDtoResponse"/>)
    /// ou erro caso o saldo não seja encontrado.
    /// </returns>
    public async Task<Result<CarteiraDto.CarteiraDtoResponse>> ConsultaSaldos(Guid UsuarioId) {
        var carteira = await carteiraRepository.ObtemSaldoPorId(UsuarioId);

        if (carteira == null) {
            await logger.LogError($"Saldo não encontrado.");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("Saldo não encontrado.", "500");
        }

        await logger.LogInformation("Finalizou ConsultaSaldos");
        return Result.Success(new CarteiraDto.CarteiraDtoResponse { Saldo = carteira.Saldo });
    }

    /// <summary>
    /// Insere ou atualiza o saldo da carteira de um usuário.
    /// </summary>
    /// <param name="request">Dados para inserção ou atualização do saldo (ID do usuário e valor do saldo).</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> contendo o saldo atualizado (<see cref="CarteiraDto.CarteiraDtoResponse"/>).
    /// </returns>
    public async Task<Result<CarteiraDto.CarteiraDtoResponse>> InsereSaldos(CarteiraDto.CarteiraDtoRequest request) {
        var carteira = await carteiraRepository.ObtemSaldoPorId(request.PerfilId);
        if (carteira != null) {
            var novaCarteira = CarteiraEnt.Atualizar(
                carteira.Id,
                carteira.PerfilId,
                carteira.Saldo + request.Saldo);

            carteiraRepository.AlteraSaldo(novaCarteira);

            return Result.Success(new CarteiraDto.CarteiraDtoResponse {
                Saldo = novaCarteira.Saldo
            });
        }

        var saldo = CarteiraEnt.Criar(request.PerfilId, request.Saldo);

        await carteiraRepository.AdicionaSaldo(saldo);

        await carteiraRepository.Commit();

        await logger.LogInformation("Finalizou InserirSaldos");

        return Result.Success(new CarteiraDto.CarteiraDtoResponse {
            Saldo = saldo.Saldo
        });
    }

    public async Task<Result<CarteiraDto.CarteiraDtoResponse>> RemoverSaldos(CarteiraDto.CarteiraDtoRequest request) {
        
        // Validações de entrada
        if (request == null)
        {
            await logger.LogError("Request de remoção de saldo é nulo");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("Request inválido.", "400");
        }

        if (request.PerfilId == Guid.Empty)
        {
            await logger.LogError("PerfilId inválido na remoção de saldo");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("PerfilId inválido.", "400");
        }

        if (request.Saldo <= 0)
        {
            await logger.LogError($"Valor de saldo inválido: {request.Saldo}");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("Valor a remover deve ser maior que zero.", "400");
        }

        // Buscar carteira
        var carteira = await carteiraRepository.ObtemSaldoPorId(request.PerfilId);
        if (carteira == null) {
            await logger.LogError($"Carteira não encontrada para o perfil {request.PerfilId}");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("Carteira não encontrada.", "404");
        }

        // Validar saldo disponível
        if (carteira.Saldo == 0) {
            await logger.LogWarning($"Tentativa de remover saldo de carteira com saldo zero: {request.PerfilId}");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("Saldo já está zerado.", "400");
        }

        if (carteira.Saldo < request.Saldo) {
            await logger.LogWarning(
                $"Tentativa de remover saldo maior que o disponível. Disponível: {carteira.Saldo}, Solicitado: {request.Saldo}");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>(
                $"Saldo insuficiente. Você possui {carteira.Saldo:C} e está tentando remover {request.Saldo:C}.", "400");
        }

        // Calcular novo saldo
        var novoSaldo = carteira.Saldo - request.Saldo;
        
        // Garantir que não fique negativo (proteção adicional)
        if (novoSaldo < 0) {
            novoSaldo = 0;
            await logger.LogWarning($"Saldo calculado ficaria negativo, ajustando para zero: {request.PerfilId}");
        }

        var novaCarteira = CarteiraEnt.Atualizar(
            carteira.Id,
            carteira.PerfilId,
            novoSaldo);

        carteiraRepository.AlteraSaldo(novaCarteira);
        await carteiraRepository.Commit();

        await logger.LogInformation(
            $"Saldo removido com sucesso. Perfil: {request.PerfilId}, Valor removido: {request.Saldo:C}, Novo saldo: {novoSaldo:C}");

        return Result.Success(new CarteiraDto.CarteiraDtoResponse {
            Saldo = novaCarteira.Saldo
        });
    }
}