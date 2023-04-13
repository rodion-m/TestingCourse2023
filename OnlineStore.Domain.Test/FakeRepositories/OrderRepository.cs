using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Test.FakeRepositories;

public class OrderRepository : InMemoryRepository<Order>, IOrderRepository
{
    public Task<Order> GetByAccountId(Guid accountId, CancellationToken cancellationToken = default)
    {
        var order = Entities.Single(it => it.AccountId == accountId);
        return Task.FromResult(order);
    }

    public Task<Order?> FindByAccountId(Guid accountId, CancellationToken cancellationToken = default)
    {
        var order = Entities.FirstOrDefault(it => it.AccountId == accountId);
        return Task.FromResult(order);
    }
}