using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Common.Repositories;

public interface IGenericRepository<TAggr, in TId> where TAggr: AggregateRoot 
{
    Task<Result<TAggr>> GetAsync(TId id);
    Task<Result> RemoveAsync(TId id);
    Task<Result> AddAsync(TAggr aggregate);
}