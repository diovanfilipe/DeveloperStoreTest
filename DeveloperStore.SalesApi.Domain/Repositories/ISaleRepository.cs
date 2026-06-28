using DeveloperStore.SalesApi.Domain.Entities;

namespace DeveloperStore.SalesApi.Domain.Repositories;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync(CancellationToken cancellationToken);

    Task<Sale?> GetByIdAsync(Guid saleId, CancellationToken cancellationToken);

    Task<Sale?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken);

    Task<Sale> CreateAsync(Sale sale, string idempotencyKey, CancellationToken cancellationToken);

    Task UpdateAsync(Sale sale, CancellationToken cancellationToken);
}
