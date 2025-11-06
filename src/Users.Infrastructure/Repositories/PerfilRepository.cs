using Microsoft.EntityFrameworkCore;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories;

public class PerfilRepository : IPerfilRepository
{
    private readonly DbUser _dbUser;

    public PerfilRepository(DbUser dbUser) {
        _dbUser = dbUser;
    }

    public async Task AtualizaPerfil(PerfilEnt perfil) {
        _dbUser.USERS.Update(perfil);
        await Commit();
    }


    public async Task Commit() =>
        await _dbUser.SaveChangesAsync();

    public async Task CriarPerfil(PerfilEnt usuario) =>
        await _dbUser.USERS.
        AddAsync(usuario);

    public async Task<PerfilEnt> TrazUsuario(string email) =>
        await _dbUser.USERS.
        AsNoTracking().
        FirstOrDefaultAsync(x => x.Email == email && x.Habilitado == 1);


}
