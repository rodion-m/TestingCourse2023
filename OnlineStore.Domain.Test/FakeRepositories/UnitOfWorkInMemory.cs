using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Test.FakeRepositories;

/// <summary>
/// This class is NOT thread-safe.
/// </summary>
public class UnitOfWorkInMemory : IUnitOfWork
{
    public IAccountRepository AccountRepository => new AccountRepository();
    public ICartRepository CartRepository => new CartRepository();
    public IProductRepository ProductRepository => new ProductRepository();
    public ICategoryRepository CategoryRepository => new CategoryRepository();
    public IOrderRepository OrderRepository => new OrderRepository();
    
    
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Имитируем сохранение в БД
        return Task.CompletedTask;
    }
}