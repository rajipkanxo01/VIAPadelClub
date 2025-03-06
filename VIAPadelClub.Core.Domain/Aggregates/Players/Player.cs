using VIAPadelClub.Core.Domain.Aggregates.Players.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players;

public class Player : AggregateRoot
{
    internal Email email;
    internal FullName fullName;
    internal ProfileUri url;
    internal VIPMemberShip vipMemberShip = new();
    internal bool isQuarantined = false;
    internal Quarantine? quarantine = new ();
    internal int quarantineId = 0;
    internal ActiveBooking activeBooking = new();
    internal bool isBlackListed = false;
    internal List<Quarantine> quarantines = new();

    private Player(Email email, FullName fullName, ProfileUri url)
    {
        this.email = email;
        this.fullName = fullName;
        this.url = url;
    }
    
    public static Result<Player> Register(string email, string firstName, string lastName, string profileUri)
    {
        var emailResult = Email.Create(email.ToLower());
        if (!emailResult.Success) return Result<Player>.Fail(emailResult.ErrorMessage);

        var fullNameResult = FullName.Create(firstName, lastName);
        if (!fullNameResult.Success) return Result<Player>.Fail(fullNameResult.ErrorMessage);

        var profileUriResult = ProfileUri.Create(profileUri);
        if (!profileUriResult.Success) return Result<Player>.Fail(profileUriResult.ErrorMessage);
    
        return Result<Player>.Ok(new Player(emailResult.Data, fullNameResult.Data, profileUriResult.Data));
    }

    public Result Blacklist()
    {
        if (isBlackListed) return Result.Fail("Player Already Blacklisted.");
        
        isBlackListed = true;
        if (quarantine is not null) quarantine = null;

        return Result.Ok();
    }

    public Result<Quarantine> Quarantine(DateTime startDate, TimeSpan duration)
    {
        throw new NotImplementedException();
    }
}
