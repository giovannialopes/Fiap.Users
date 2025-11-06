using Users.Domain.Entity;

namespace Users.Domain.Repositories;

public interface ICarteiraRepository : ICommit
{
    Task<CarteiraEnt> ObtemSaldoPorId(Guid usuarioId);
    Task AdicionaSaldo(CarteiraEnt carteira);
    void AlteraSaldo(CarteiraEnt carteira);

}
