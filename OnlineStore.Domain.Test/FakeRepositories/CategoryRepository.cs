using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Test.FakeRepositories;

public class CategoryRepository : InMemoryRepository<Category>, ICategoryRepository
{
    public async Task<Category> GetByName(string name, CancellationToken cancellationToken = default)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return Entities.First(it => it.Name == name);
    }
}