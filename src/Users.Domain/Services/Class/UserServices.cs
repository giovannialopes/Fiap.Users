using Users.Domain.DTO;
using Users.Domain.Enum;
using Users.Domain.Repositories;
using Users.Domain.Results;
using Users.Domain.Services.Interface;
using Users.Domain.Utils;

namespace Users.Domain.Services.Class;

public class UserServices(IPerfilRepository repository, IJwtServices jwtServices, ILoggerServices logger) : IUserServices
{
    public async Task<Result<LoginDto.LoginDtoResponse>> Entrar(LoginDto.LoginDtoRequest request) {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha)) {
            await logger.LogError($"Email ou senha não informados.");
            return Result.Failure<LoginDto.LoginDtoResponse>("Email ou senha não informados.", "500");
        }

        var usuario = await repository.TrazUsuario(request.Email);
        if (usuario == null) {
            await logger.LogError($"Usuário não encontrado.");
            return Result.Failure<LoginDto.LoginDtoResponse>("Usuário não encontrado.", "500");
        }

        if (!ValidarSenha.Validar(request.Senha, usuario.Senha)) {
            await logger.LogError($"Senha inválida.");
            return Result.Failure<LoginDto.LoginDtoResponse>("Senha inválida.", "500");
        }

        var token = jwtServices.GenerateToken(usuario.Id, PerfilEnum.Administrador.ToString());

        var response = new LoginDto.LoginDtoResponse {
            Id = usuario.Id,
            Token = token
        };

        await logger.LogInformation("Finalizou EntrarNoSistema");
        return Result.Success(response);
    }
}
