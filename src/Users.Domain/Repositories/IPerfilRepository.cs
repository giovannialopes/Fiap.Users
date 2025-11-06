using Users.Domain.Entity;

namespace Users.Domain.Repositories;

public interface IPerfilRepository : ICommit
{
    Task CriarPerfil(PerfilEnt usuario);
    Task<PerfilEnt> TrazUsuario(string email);
    Task AtualizaPerfil(PerfilEnt perfil);


}
