using Users.Domain.Queue.Event;

namespace Users.Domain.Queue;

/// <summary>
/// Interface para publicação de eventos relacionados à carteira no RabbitMQ.
/// Pode ser usado no futuro para publicar eventos quando o saldo for atualizado.
/// </summary>
public interface IWalletBusPublisher
{
    /// <summary>
    /// Publica um evento de atualização de carteira.
    /// </summary>
    /// <param name="event">Evento de atualização de carteira</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task Publish(WalletUpdatedEvent @event, CancellationToken cancellationToken = default);
}

