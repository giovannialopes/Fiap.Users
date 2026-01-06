namespace Users.Domain.Queue.Event;

/// <summary>
/// Evento publicado quando o saldo da carteira de um usuário é atualizado.
/// Pode ser usado no futuro para notificar outros microserviços sobre mudanças no saldo.
/// </summary>
public record WalletUpdatedEvent(
    Guid PerfilId,
    decimal Saldo,
    decimal SaldoAnterior,
    string TipoOperacao // "Adicionado", "Removido", "Atualizado"
);

