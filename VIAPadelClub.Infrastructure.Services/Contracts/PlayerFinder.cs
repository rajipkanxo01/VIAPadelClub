using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace Services.Contracts;

public class PlayerFinder(IPlayerRepository playerRepository) : IPlayerFinder
{
    public Result<Player> FindPlayer(Email email)
    {
        var player = playerRepository.GetAsync(email).Result;
        return player;
    }

    public Result AddPlayer(Player player)
    {
        var result = playerRepository.AddAsync(player).Result;
        return result;
    }
}