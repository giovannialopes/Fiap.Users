using Users.Domain.DTO;
using Users.Domain.Results;

namespace Users.Domain.Services.Interface;

public interface IUserServices
{
    Task<Result<LoginDto.LoginDtoResponse>> Entrar(LoginDto.LoginDtoRequest request);

}
