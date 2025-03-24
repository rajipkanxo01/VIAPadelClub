using System.Runtime.CompilerServices;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
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
    
    public Email Email => email;
    
    public static async Task<Result<Player>> Register(Email email, FullName fullName, ProfileUri profileUri,IEmailUniqueChecker emailUniqueChecker)
    {
        if (!await emailUniqueChecker.IsUnique(email.Value.ToLower()))
        {
            return Result<Player>.Fail(DailyScheduleError.DuplicateEmail()._message);
        }
        
        emailUniqueChecker.AddEmail(email.Value.ToLower());
        var playerResult = new Player(email, fullName, profileUri);
        return Result<Player>.Ok(playerResult);
    }
    
    public Result<Quarantine> Quarantine(DateOnly startDate, List<DailySchedule> schedules)
    {
        if (isBlackListed)
            return Result<Quarantine>.Fail(PlayerError.BlackListedCannotQuarantine()._message);

        var quarantine = Entities.Quarantine.CreateOrExtend(quarantines, startDate);
    
        if (!quarantines.Contains(quarantine))
        {
            quarantines.Add(quarantine);
            isQuarantined = true;
            activeQuarantine=quarantine;
        }
        
        //TODO: Uncomment below once Player entity is available
        //CancelBookingsDuringQuarantine(schedules, quarantine);
        
        return Result<Quarantine>.Ok(quarantine);
    }
    
    //TODO: TODO: Uncomment below once Cancellation of booking and Player entity is available 
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
    //             Console.WriteLine($"Player for {booking.court.courtName.Value} on {booking.startTime} canceled due to quarantine.");
    //             schedule.cancelBooking(booking.bookingId);
    //         }
    //     }
    // }

    public Result Blacklist(IScheduleFinder scheduleFinder)
    {
        if (isBlackListed) return Result.Fail(DailyScheduleError.PlayerAlreadyBlacklisted()._message);
        
        isBlackListed = true;
        if (activeQuarantine is not null) activeQuarantine = null;

        // TODO: remove all bookings!!!
        
        return Result.Ok();
    }

    public Result LiftBlacklist()
    {
        if (!isBlackListed) return Result.Fail(DailyScheduleError.PlayerIsNotBlacklisted()._message);

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