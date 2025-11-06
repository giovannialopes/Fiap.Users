using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Users.Domain.Config;
using Users.Domain.Services.Interface;

namespace Users.Domain.Services.Class;

public class JwtServices : IJwtServices
{
    private readonly JwtSettings _settings;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="JwtServices"/> com as configurações de JWT.
    /// </summary>
    /// <param name="settings">Configurações de JWT injetadas via <see cref="IOptions{JwtSettings}"/>.</param>
    public JwtServices(IOptions<JwtSettings> settings) {
        _settings = settings.Value;
    }

    /// <summary>
    /// Gera um token JWT para o usuário informado.
    /// </summary>
    /// <param name="userId">Identificador único do usuário (Guid).</param>
    /// <param name="role">Papel (role) do usuário para autorização.</param>
    /// <returns>Token JWT assinado, pronto para ser utilizado em autenticação.</returns>
    /// <remarks>
    /// O token inclui as claims: Sub (userId), Role e Jti (identificador único do token).
    /// A expiração, chave, emissor e audiência são definidos pelas configurações de <see cref="JwtSettings"/>.
    /// </remarks>
    public string GenerateToken(Guid userId, string role) {
        var claims = new List<Claim>
        {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiresInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
