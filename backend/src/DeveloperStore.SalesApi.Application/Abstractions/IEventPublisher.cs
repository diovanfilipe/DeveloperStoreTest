namespace DeveloperStore.SalesApi.Application.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync(string eventName, Guid saleId, string description, CancellationToken cancellationToken);
}
