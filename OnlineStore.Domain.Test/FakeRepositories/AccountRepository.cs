using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Test.FakeRepositories;

public class AccountRepository : InMemoryRepository<Account>, IAccountRepository
{
    public async Task<Account> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email));
        }
        return Entities.First(it => it.Email == email);
    }

    public async Task<Account?> FindByEmail(string email, CancellationToken cancellationToken = default)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email));
        }
        return Entities.FirstOrDefault(it => it.Email == email);
    }
}