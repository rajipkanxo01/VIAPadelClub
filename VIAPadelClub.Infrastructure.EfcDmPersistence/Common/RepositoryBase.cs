using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Domain.Common.Repositories;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Common;

public class RepositoryBase<TAggr, TId>(DbContext context) : IGenericRepository<TAggr, TId> where TAggr : AggregateRoot
{
    public static Error EntityNotFound() => new Error(0, "Entity not found in the database");

    public virtual async Task<Result<TAggr>> GetAsync(TId id)
    {
        var root = await context.Set<TAggr>().FindAsync(id);

        if (root is null)
        {
            return Result<TAggr>.Fail(EntityNotFound()._message);
        }

        return Result<TAggr>.Ok(root);
    }

    public virtual async Task<Result> RemoveAsync(TId id)
    {
        var root = await context.Set<TAggr>().FindAsync(id);

        if (root is null)
        {
            return Result.Fail(EntityNotFound()._message);
        }

        context.Set<TAggr>().Remove(root);

        return Result.Ok();
    }

    public virtual async Task<Result> AddAsync(TAggr aggregate)
    {
        await context.Set<TAggr>().AddAsync(aggregate);
        return Result.Ok();
    }
}