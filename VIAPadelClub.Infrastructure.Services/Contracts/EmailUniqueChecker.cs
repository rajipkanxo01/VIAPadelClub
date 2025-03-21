using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace Services.Contracts;

public class EmailUniqueChecker: IEmailUniqueChecker
{
    private static readonly HashSet<string> Emails = new();

    public Task<bool> IsUnique(string email)
    {
        return Task.FromResult(!Emails.Contains(email.ToLower()));
    }

    public void AddEmail(string email)
    {
        Emails.Add(email.ToLower());
    }
}