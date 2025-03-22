using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers;

public class FakePlayerFinder: IPlayerFinder
{
    private readonly List<Player> _players = new();
    public Result<Player> FindPlayer(Email email)
    {
        var player = _players.FirstOrDefault(p => p.email.Value.Equals(email.Value));
        return player != null ? Result<Player>.Ok(player) : Result<Player>.Fail(ErrorMessage.NoPlayerFound()._message);
    }

    public Result AddPlayer(Player player)
    {
        _players.Add(player);
        return Result.Ok();
    }
}