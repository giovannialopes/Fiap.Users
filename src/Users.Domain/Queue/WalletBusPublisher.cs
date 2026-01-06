using MassTransit;
using Microsoft.Extensions.Logging;
using Users.Domain.Queue.Event;

namespace Users.Domain.Queue;

/// <summary>
/// Implementação do publisher de eventos de carteira no RabbitMQ.
/// </summary>
public class WalletBusPublisher : IWalletBusPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<WalletBusPublisher> _logger;
    
    public WalletBusPublisher(IPublishEndpoint publishEndpoint, ILogger<WalletBusPublisher> logger) 
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Publish(WalletUpdatedEvent @event, CancellationToken cancellationToken = default) 
    {
        try
        {
            _logger.LogInformation("Publicando evento WalletUpdatedEvent: PerfilId={PerfilId}, Saldo={Saldo}, TipoOperacao={TipoOperacao}", 
                @event.PerfilId, @event.Saldo, @event.TipoOperacao);
            
            await _publishEndpoint.Publish(@event, cancellationToken);
            
            _logger.LogInformation("Evento WalletUpdatedEvent publicado com sucesso: PerfilId={PerfilId}", 
                @event.PerfilId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar evento WalletUpdatedEvent: PerfilId={PerfilId}", 
                @event.PerfilId);
            throw;
        }
    }
}

