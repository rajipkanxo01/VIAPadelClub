using VIAPadelClub.Core.Domain.Aggregates.Players;

namespace UnitTests.Features.Helpers;

public class FakeUniqueEmailChecker: IEmailUniqueChecker
{
    private readonly HashSet<string> _emails = new HashSet<string>();

    public Task<bool> IsUnique(string email)
    {
        return Task.FromResult(!_emails.Contains(email.ToLower()));
    }

    public void AddEmail(string email)
    {
        _emails.Add(email.ToLower());
    }
}