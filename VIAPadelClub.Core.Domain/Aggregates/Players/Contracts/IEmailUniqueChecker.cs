namespace VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;

public interface IEmailUniqueChecker
{
    Task<bool> IsUnique(string email);
    void AddEmail(string email);// Todo: Remove AddEmail after session 6
}