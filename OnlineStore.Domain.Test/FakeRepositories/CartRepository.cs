using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Test.FakeRepositories;

public class CartRepository : InMemoryRepository<Cart>, ICartRepository
{
    public async Task<Cart> GetByAccountId(Guid accountId, CancellationToken cancellationToken = default)
    {
        var cart = Entities.SingleOrDefault(it => it.AccountId == accountId);
        return cart;
    }

    public async Task<Cart?> FindByAccountId(Guid accountId, CancellationToken cancellationToken = default)
    {
        var cart = Entities.FirstOrDefault(it => it.AccountId == accountId);
        return cart;
    }

    public async Task<CartItem> GetItemById(Guid id, Guid accountId, CancellationToken cancellationToken = default)
    {
        var cart = await GetByAccountId(accountId, cancellationToken);
        var cartItem = cart.Items.SingleOrDefault(it => it.Id == id);
        if (cartItem == null)
        {
            throw new NoSuchItemCartException($"There is no such item with {id} in Cart");
        }

        return cartItem;
    }
}