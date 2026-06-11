using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Repository.IRepository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ArmazemCalabria.Repository.Imp.Repository.Base;

public abstract class RepositoryBase<TEntity>(Context context) : IRepositoryBase<TEntity> where TEntity : class, IEntity
{
    protected Context Context { get; } = context;

    protected readonly DbSet<TEntity> Entity = context.Set<TEntity>();

    public virtual async Task<TEntity> InsertAsync(TEntity entidade, CancellationToken cancellationToken = default)
    {
        Context.Add(entidade);
        await Context.SaveChangesAsync(cancellationToken);

        return entidade;
    }

    public async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entidades, CancellationToken cancellationToken = default)
    {
        Context.AddRange(entidades);
        await Context.SaveChangesAsync(cancellationToken);

        return entidades;
    }

    public async Task UpdateAsync(TEntity entidade, CancellationToken cancellationToken = default)
    {
        Context.Update(entidade);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(IEnumerable<TEntity> entidades, CancellationToken cancellationToken = default)
    {
        Context.UpdateRange(entidades);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(TEntity entidade, CancellationToken cancellationToken = default)
    {
        Context.Remove(entidade);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(IEnumerable<TEntity> entidades, CancellationToken cancellationToken = default)
    {
        Context.RemoveRange(entidades);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return Entity.AsQueryable();
    }

    public async virtual Task<TEntity> FilterById(object id)
    {
        return await Entity.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> ObterTodosAsNoTracking()
    {
        return await Entity.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> ObterTodos()
    {
        return await Entity.ToListAsync();
    }

    public void RemoveEntityContext(object id)
    {
        var existingEntity = Entity.Find(id);

        if (existingEntity != null)
        {
            Context.Entry(existingEntity).State = EntityState.Detached;
        }
    }

    public IEnumerable<Tprop> GetPropEntityBy<Tprop>(Func<TEntity, bool> predicate, Func<TEntity, Tprop> predicatePropReturn)
    {
        return Entity.Where(predicate)
                     .Select(predicatePropReturn)
                     .ToList();
    }

    public IQueryable<TEntity> AgregarIncludes(Expression<Func<TEntity, object>>[] includes)
    {
        return includes.Aggregate(Entity.AsQueryable(), (current, include) => current.Include(include));
    }
}
