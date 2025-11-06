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
        var carteira = await carteiraRepository.ObtemSaldoPorId(request.PerfilId);
        if (carteira == null) {
            await logger.LogError($"Carteira não encontrada.");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("Carteira não encontrado.", "500");
        }

        if (carteira.Saldo == 0) {
            await logger.LogError($"Saldo já removido.");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("Saldo já removido.", "500");
        }



        var novaCarteira = CarteiraEnt.Atualizar(
            carteira.Id,
            carteira.PerfilId,
            carteira.Saldo - request.Saldo);

        if (novaCarteira.Saldo.ToString().Contains("-")) {
            novaCarteira.Saldo = 0;

            await logger.LogError($"Saldo negativo.");
            return Result.Failure<CarteiraDto.CarteiraDtoResponse>("Você não possui saldo suficiente.", "500");
        }

        carteiraRepository.AlteraSaldo(novaCarteira);

        return Result.Success(new CarteiraDto.CarteiraDtoResponse {
            Saldo = novaCarteira.Saldo
        });
    }
}