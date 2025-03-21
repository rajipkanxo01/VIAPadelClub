using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players;

public interface IPlayerRepository
{
    Task<Result> AddAsync();
    Task<Result> GetAsync();
    Task<Result> RemoveAsync();
}