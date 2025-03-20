using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players;

public interface IEmailUniqueChecker
{
    Task<bool> IsUnique(string email);
    void AddEmail(string email);// Todo: Remove AddEmail after session 6
}

public interface IPlayerFinder
{
    Result<Player> FindPlayer(string email);
    void AddPlayer(Player player);    //Todo: Remove Add player after session 6
}