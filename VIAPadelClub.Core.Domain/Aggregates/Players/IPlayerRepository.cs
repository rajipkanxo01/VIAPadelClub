using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players;

public interface IPlayerRepository
{
    Task<Result> AddAsync(Player player);
    Task<Result<Player>> GetAsync(Email playerEmail);
    Task<Result> RemoveAsync();
}