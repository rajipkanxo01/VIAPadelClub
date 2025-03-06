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
    internal VIPMemberShip vipMemberShip;
    internal bool isQuarantined;
    internal int quarantineId;
    internal ActiveBooking activeBooking;
    internal bool isBlackListed;
    internal List<Quarantine> quarantines;

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
        
        return Result<Player>.Ok(new Player(emailResult.Data!, fullNameResult.Data!, profileUriResult.Data!));
    }
}
