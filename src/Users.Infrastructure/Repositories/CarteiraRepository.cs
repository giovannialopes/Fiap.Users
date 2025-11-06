using Microsoft.EntityFrameworkCore;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories;

public class CarteiraRepository : ICarteiraRepository
{
    private readonly DbUser _dbUser;
    public CarteiraRepository(DbUser dbUser) {
        _dbUser = dbUser;
    }


    public async Task Commit() =>
    await _dbUser.
    SaveChangesAsync();

    public async Task AdicionaSaldo(CarteiraEnt carteira) => await _dbUser.CARTEIRA.AddAsync(carteira);

    public void AlteraSaldo(CarteiraEnt carteira) {
        _dbUser.CARTEIRA.Update(carteira);
        _dbUser.SaveChanges();
    }

    public async Task<CarteiraEnt> ObtemSaldoPorId(Guid perfilId) =>
        await _dbUser.CARTEIRA.AsNoTracking().FirstOrDefaultAsync(x => x.PerfilId == perfilId);
}
