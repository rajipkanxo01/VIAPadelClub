using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace Services.Contracts;

public class PlayerFinder
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerFinder(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Result<Player> FindPlayer(Email email)
    {
        var player = _playerRepository.GetAsync(email).Result;
        return player;
    }

    public Result AddPlayer(Player player)
    {
        var result = _playerRepository.AddAsync(player).Result;
        return result;
    }
}