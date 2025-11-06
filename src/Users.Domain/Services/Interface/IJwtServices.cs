namespace Users.Domain.Services.Interface;

public interface IJwtServices
{
    string GenerateToken(Guid userId, string role);

}
