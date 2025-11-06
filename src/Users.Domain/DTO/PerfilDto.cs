using Users.Domain.Enum;

namespace Users.Domain.DTO;

public class PerfilDto
{
    public class CriarPerfilRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
    }

    public class PerfilRequest
    { 
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public PerfilEnum PerfilEnum { get; set; } 
    }

    public class PerfilResponse
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
    }


}
