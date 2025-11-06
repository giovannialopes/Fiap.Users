using Users.Domain.DTO;
using Users.Domain.Entity;
using Users.Domain.Enum;
using Users.Domain.Repositories;
using Users.Domain.Results;
using Users.Domain.Services.Interface;
using Users.Domain.Utils;

namespace Users.Domain.Services.Class;

public class PerfilServices(IPerfilRepository repository, IJwtServices jwtServices, ILoggerServices logger) : IPerfilServices
{
    /// <summary>
    /// Cria um novo perfil de usuário.
    /// </summary>
    /// <param name="request">Dados do perfil a ser criado.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> contendo o ID do perfil criado e o token JWT (<see cref="PerfilDto.PerfilResponse"/>),
    /// ou erro caso ocorra alguma falha na criação.
    /// </returns>
    public async Task<Result<PerfilDto.PerfilResponse>> CriarPerfil(PerfilDto.CriarPerfilRequest request) {

        var perfil = PerfilEnt.Criar(
            request.Nome,
            request.Email,
            request.SenhaHash,
            PerfilEnum.Usuario
        );

        try {
            await repository.CriarPerfil(perfil);
            await repository.Commit();

            var token = jwtServices.GenerateToken(perfil.Id, PerfilEnum.Usuario.ToString());

            await logger.LogInformation("Finalizou CriarPerfil");
            return Result.Success(new PerfilDto.PerfilResponse {
                Id = perfil.Id,
                Token = token
            });
        }
        catch (Exception ex) {
            return Result.Failure<PerfilDto.PerfilResponse>($"Erro ao criar perfil usuário", "500");
        }
    }

    /// <summary>
    /// Altera os dados de um perfil de usuário existente.
    /// </summary>
    /// <param name="email">E-mail do usuário a ser alterado.</param>
    /// <param name="senha">Senha do usuário para validação.</param>
    /// <param name="request">Novos dados do perfil.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> contendo mensagem de sucesso ou erro caso as credenciais estejam incorretas ou o usuário não seja encontrado.
    /// </returns>
    public async Task<Result<string>> AlterarPerfil(string email, string senha, PerfilDto.PerfilRequest request) {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha)) {
            await logger.LogError($"Email ou senha não informados.");
            return Result.Failure<string>("Email ou senha não informados.", "500");
        }

        var usuario = await repository.TrazUsuario(email);
        if (usuario == null) {
            await logger.LogError($"Usuário não encontrado para alteração.");
            return Result.Failure<string>("Usuário não encontrado para alteração.", "500");
        }

        if (!ValidarSenha.Validar(senha, usuario.Senha)) {
            await logger.LogError($"Senha inválida para alteração.");
            return Result.Failure<string>("Senha inválida para alteração.", "500");
        }

        usuario.Nome = request.Nome;
        usuario.Email = request.Email;
        usuario.Senha = BCrypt.Net.BCrypt.HashPassword(request.SenhaHash);
        usuario.Perfil = request.PerfilEnum;
        usuario.DataAlteracao = DateTime.UtcNow;

        await repository.AtualizaPerfil(usuario);
        await logger.LogInformation("Finalizou AlterarPerfil");
        return Result.Success("Perfil atualizado com sucesso.");
    }

    /// <summary>
    /// Desativa (deleta logicamente) um perfil de usuário.
    /// </summary>
    /// <param name="email">E-mail do usuário a ser desativado.</param>
    /// <param name="senha">Senha do usuário para validação.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> contendo mensagem de sucesso ou erro caso as credenciais estejam incorretas ou o usuário não seja encontrado.
    /// </returns>
    public async Task<Result<string>> DeletarPerfil(string email, string senha) {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha)) {
            await logger.LogError($"Email ou senha não informados.");
            return Result.Failure<string>("Email ou senha não informados.", "500");
        }

        var usuario = await repository.TrazUsuario(email);
        if (usuario == null) {
            await logger.LogError($"Usuário não encontrado para alteração.");
            return Result.Failure<string>("Usuário não encontrado para alteração.", "500");
        }

        if (!ValidarSenha.Validar(senha, usuario.Senha)) {
            await logger.LogError($"Senha inválida para alteração.");
            return Result.Failure<string>("Senha inválida para alteração.", "500");
        }

        usuario.Habilitado = 0;
        usuario.DataAlteracao = DateTime.UtcNow;

        await repository.AtualizaPerfil(usuario);
        await logger.LogInformation("Finalizou DeletarPerfil");
        return Result.Success("Perfil desativado com sucesso.");
    }


}
