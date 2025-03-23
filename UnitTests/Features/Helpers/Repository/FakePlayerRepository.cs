using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers.Repository;

public class FakePlayerRepository : IPlayerRepository
{
    private List<Player> _players = new();
    public Task<Result> AddAsync(Player player)
    {
       _players.Add(player);
       return Task.FromResult(Result.Ok());
    }

    public Task<Result<Player>> GetAsync(Email playerEmail)
    {
        var player = _players.FirstOrDefault(p => p.email.Value.Equals(playerEmail.Value));
        return Task.FromResult(Result<Player>.Ok(player));
    }

    public Task<Result> RemoveAsync()
    {
        throw new NotImplementedException();
    }
}