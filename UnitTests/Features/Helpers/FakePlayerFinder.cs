using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers;

public class FakePlayerFinder: IPlayerFinder
{
    private readonly List<Player> _players = new();
    public Result<Player> FindPlayer(string email)
    {
        var player = _players.FirstOrDefault(p => p.email.Value.Equals(email));
        return player != null ? Result<Player>.Ok(player) : Result<Player>.Fail(ErrorMessage.NoPlayerFound()._message);
    }

    public void AddPlayer(Player player)
    {
        _players.Add(player);
    }
}