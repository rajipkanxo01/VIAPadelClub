namespace VIAPadelClub.Core.Domain.Aggregates.Players.Entities;

using Tools.OperationResult;

public class VIPMemberShip {
    internal DateOnly StartDate { get; }
    internal DateOnly EndDate { get; }
    internal bool IsVIP { get; private set; }
    
    private VIPMemberShip(DateOnly startDate, DateOnly endDate, bool isVip)
    {
        StartDate = startDate;
        EndDate = endDate;
        IsVIP = isVip;
    }

    public static Result<VIPMemberShip> Create(VIPMemberShip? currentMemberShip)
    {
        if (currentMemberShip != null && currentMemberShip.IsVIP)
            return Result<VIPMemberShip>.Fail("Player is already a VIP.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return Result<VIPMemberShip>.Ok(new VIPMemberShip(today, today.AddDays(30), true));
    }
    
    public bool HasExpired()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return today > EndDate;
    }

    public void ExpireStatus()
    {
        IsVIP = false;
    }
}