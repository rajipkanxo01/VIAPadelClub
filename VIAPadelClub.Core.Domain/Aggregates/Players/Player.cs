using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

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
    internal bool isQuarantined = false;
    internal Quarantine? activeQuarantine;
    internal int quarantineId = 0;
    internal ActiveBooking activeBooking = new();
    internal bool isBlackListed = false;
    internal List<Quarantine> quarantines;

    private Player(Email email, FullName fullName, ProfileUri url)
    {
        this.email = email;
        this.fullName = fullName;
        this.url = url;
        quarantines = new List<Quarantine>();
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

    public Result<Quarantine> Quarantine(DateOnly startDate, List<DailySchedule> schedules)
    {
        if (isBlackListed)
            return Result<Quarantine>.Fail("Player is already blacklisted and cannot be quarantined.");

        var quarantine = Entities.Quarantine.CreateOrExtend(quarantines, startDate);
    
        if (!quarantines.Contains(quarantine))
        {
            quarantines.Add(quarantine);
            isQuarantined = true;
        }
        
        //TODO: Uncomment below once Booking entity is available
        //CancelBookingsDuringQuarantine(schedules, quarantine);
        
        return Result<Quarantine>.Ok(quarantine);
    }
    
    //TODO: TODO: Uncomment below once Cancellation of booking and Booking entity is available 
    // private void CancelBookingsDuringQuarantine(List<DailySchedule> schedules, Quarantine quarantine)
    // {
    //     foreach (var schedule in schedules)
    //     {
    //         var bookingsToCancel = schedule.listOfBookings
    //             .Where(b => b.bookedBy.email == this.email && 
    //                         b.startTime.Date >= quarantine.StartDate && 
    //                         b.startTime.Date <= quarantine.EndDate)
    //             .ToList();
    //
    //         foreach (var booking in bookingsToCancel)
    //         {
    //             Console.WriteLine($"Booking for {booking.court.courtName.Value} on {booking.startTime} canceled due to quarantine.");
    //             schedule.cancelBooking(booking.bookingId);
    //         }
    //     }
    // }

    public Result Blacklist(List<DailySchedule> dailySchedules)
    {
        if (isBlackListed) return Result.Fail("Player Already Blacklisted. Cannot blacklist same player twice!!");
        
        isBlackListed = true;
        if (activeQuarantine is not null) activeQuarantine = null;

        return Result.Ok();
    }

    public Result LiftBlacklist()
    {
        if (!isBlackListed) return Result.Fail("Player is not blacklisted.");

        isBlackListed = false;
        return Result.Ok();
    }

    public Result ChangeToVIPStatus()
    {
        if (isBlackListed)
            return Result.Fail("Blacklisted players cannot be elevated to VIP status.");

        if (isQuarantined)
            return Result.Fail("Quarantined players cannot be elevated to VIP status.");

        var vipResult = VIPMemberShip.Create(vipMemberShip);
        
        if (!vipResult.Success)
            return Result.Fail(vipResult.ErrorMessage);

        vipMemberShip = vipResult.Data;
        
        Console.WriteLine($"**NOTIFICATION** Player {email.Value} has been upgraded to VIP!");
        return Result.Ok();
    }

    public void CheckVIPStatusExpiry()
    {
        if (vipMemberShip == null) 
            return;

        if (vipMemberShip.HasExpired())
        {
            vipMemberShip.ExpireStatus();
        }
    }
}