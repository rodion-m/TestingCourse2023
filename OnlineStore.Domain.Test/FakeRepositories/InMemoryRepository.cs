using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Test.FakeRepositories;

/// <summary>
/// Not thread safe. Only for testing purposes.
/// </summary>
public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    protected static readonly List<TEntity> Entities = new();

    public virtual async Task<TEntity> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return Entities.First(it => it.Id == id);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAll(CancellationToken cancellationToken = default)
    {
        return Entities;
    }

    public virtual async Task Add(TEntity entity, CancellationToken cancellationToken = default)
    {
        Entities.Add(entity);
    }

    public virtual ValueTask Update(TEntity entity, CancellationToken cancellationToken = default)
    {
        var index = Entities.FindIndex(e => e.Id == entity.Id);
        Entities[index] = entity;
        return default;
    }

    public virtual async Task<TEntity> DeleteById(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = Entities.First(e => e.Id == id);
        Entities.RemoveAll(e => e.Id == id);
        return entity;
    }
}