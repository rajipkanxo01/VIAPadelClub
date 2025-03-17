using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace VIAPadelClub.Core.Domain.Aggregates.Players;

public interface IEmailUniqueChecker
{
    Task<bool> IsUnique(string email);
    void AddEmail(string email);// Todo: Remove AddEmail after session 6
}