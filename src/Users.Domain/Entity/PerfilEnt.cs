using Users.Domain.Enum;

namespace Users.Domain.Entity;

public class PerfilEnt
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public PerfilEnum Perfil { get; set; } = PerfilEnum.Usuario;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAlteracao { get; set; }
    public long Habilitado { get; set; } = 1;

    public static PerfilEnt Criar(string nome, string email, string senha, PerfilEnum perfil) {
        return new PerfilEnt {
            Nome = nome,
            Email = email,
            Senha = BCrypt.Net.BCrypt.HashPassword(senha),
            Perfil = perfil,
            Id = Guid.NewGuid()
        };
    }

    public static PerfilEnt Alterar(string nome, string email, string senha, PerfilEnum perfil, Guid perfilId) {
        return new PerfilEnt {
            Nome = nome,
            Email = email,
            Senha = BCrypt.Net.BCrypt.HashPassword(senha),
            Perfil = perfil,
            Id = perfilId
        };
    }
}
