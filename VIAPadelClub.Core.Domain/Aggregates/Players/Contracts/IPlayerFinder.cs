using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;


public interface IPlayerFinder
{
    Result<Player> FindPlayer(string email);
    void AddPlayer(Player player);    //Todo: Remove Add player after session 6
}