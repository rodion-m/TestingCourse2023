using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Test.FakeRepositories;

public class ProductRepository : InMemoryRepository<Product>, IProductRepository
{
    public async Task<Product> GetByName(string name, CancellationToken cancellationToken = default)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return Entities.First(it => it.Name == name);
    }

    public async Task<IReadOnlyList<Product>> FindByName(string name, CancellationToken cancellationToken = default)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var products = Entities
            .Where(it => it.Name.Contains(name))
            .ToList();
        return products;
    }

    public async Task<IReadOnlyList<Product>> GetProductsByCategoryId(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var products = Entities
            .Where(it => it.CategoryId == categoryId)
            .ToList();
        return products;
    }
}