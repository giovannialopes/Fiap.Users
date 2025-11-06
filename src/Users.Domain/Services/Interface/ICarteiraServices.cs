using Users.Domain.DTO;
using Users.Domain.Results;

namespace Users.Domain.Services.Interface;

public interface ICarteiraServices
{
    Task<Result<CarteiraDto.CarteiraDtoResponse>> InsereSaldos(CarteiraDto.CarteiraDtoRequest request);
    Task<Result<CarteiraDto.CarteiraDtoResponse>> ConsultaSaldos(Guid UsuarioId);
    Task<Result<CarteiraDto.CarteiraDtoResponse>> RemoverSaldos(CarteiraDto.CarteiraDtoRequest request);
}
