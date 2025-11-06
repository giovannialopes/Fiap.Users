namespace Users.Domain.DTO;

public class LoginDto
{
    public class LoginDtoRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class LoginDtoResponse
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;

    }
}
