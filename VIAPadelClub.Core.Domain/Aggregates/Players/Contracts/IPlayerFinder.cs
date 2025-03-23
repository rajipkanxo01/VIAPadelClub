using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;


public interface IPlayerFinder
{
    Result<Player> FindPlayer(Email email);
    Result AddPlayer(Player player);    //Todo: Remove Add player after session 6
}