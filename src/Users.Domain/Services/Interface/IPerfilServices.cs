using Users.Domain.DTO;
using Users.Domain.Entity;
using Users.Domain.Results;

namespace Users.Domain.Services.Interface;

public interface IPerfilServices
{
    Task<Result<PerfilDto.PerfilResponse>> CriarPerfil(PerfilDto.CriarPerfilRequest request);
    Task<Result<string>> AlterarPerfil(string email, string senha, PerfilDto.PerfilRequest request);
    Task<Result<string>> DeletarPerfil(string email, string senha);
}
