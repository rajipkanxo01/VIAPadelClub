namespace VIAPadelClub.Core.Domain.Aggregates.Players.Entities;

using Tools.OperationResult;

public class VipMemberShip {
    internal DateOnly VIPStartDate { get; }
    internal DateOnly VIPEndDate { get; }
    internal bool IsVIP { get; private set; }
    
    private VipMemberShip(DateOnly vipStartDate, DateOnly vipEndDate, bool isVip)
    {
        VIPStartDate = vipStartDate;
        VIPEndDate = vipEndDate;
        IsVIP = isVip;
    }

    private VipMemberShip() // for efc
    {
    }

    public static Result<VipMemberShip> Create(VipMemberShip? currentMemberShip)
    {
        if (currentMemberShip != null && currentMemberShip.IsVIP)
            return Result<VipMemberShip>.Fail("Player is already a VIP.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return Result<VipMemberShip>.Ok(new VipMemberShip(today, today.AddDays(30), true));
    }
    
    public bool HasExpired()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return today > VIPEndDate;
    }

    public void ExpireStatus()
    {
        IsVIP = false;
    }
}