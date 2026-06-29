using DeveloperStore.SalesApi.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace DeveloperStore.SalesApi.Infrastructure.Logging;

public sealed class LogEventPublisher : IEventPublisher
{
    private readonly ILogger<LogEventPublisher> _logger;

    public LogEventPublisher(ILogger<LogEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(string eventName, Guid saleId, string description, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event {EventName} for sale {SaleId}: {Description}", eventName, saleId, description);
        return Task.CompletedTask;
    }
}
