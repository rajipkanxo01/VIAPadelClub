namespace VIAPadelClub.Core.Domain.Aggregates.Players.Entities;

using Common.BaseClasses;

public class Quarantine
{ 
    public Guid quarentineId;
    internal DateOnly StartDate { get; set; }
    internal DateOnly EndDate { get; set; }
    
    private Quarantine(DateOnly startDate)
    {
        StartDate = startDate;
        EndDate = startDate.AddDays(3);
    }
    
    public static Quarantine CreateOrExtend(List<Quarantine> quarantines, DateOnly startDate)
    {
        if (quarantines == null || quarantines.Count == 0)
        {
            return new Quarantine(startDate);
        }
        
        var latestQuarantine = quarantines.LastOrDefault(); // Get last quarantine or null

        if (latestQuarantine != null && latestQuarantine.EndDate >= startDate)
        {
            latestQuarantine.Extend();
            return latestQuarantine;
        }
        
        return new Quarantine(startDate);
    }

    private void Extend()
    {
        EndDate = EndDate.AddDays(3);
    }
}