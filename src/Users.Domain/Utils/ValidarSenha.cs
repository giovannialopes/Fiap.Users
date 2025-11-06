namespace Users.Domain.Utils;

public static class ValidarSenha
{
    public static bool Validar(string senhaFornecida, string senhaHashArmazenada) {
        if (string.IsNullOrWhiteSpace(senhaFornecida) || string.IsNullOrWhiteSpace(senhaHashArmazenada))
            return false;

        return BCrypt.Net.BCrypt.Verify(senhaFornecida, senhaHashArmazenada);
    }
}
