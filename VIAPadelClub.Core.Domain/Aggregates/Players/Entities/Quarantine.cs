namespace VIAPadelClub.Core.Domain.Aggregates.Players.Entities;

using Common.BaseClasses;

public class Quarantine
{ 
    internal DateOnly StartDate { get; set; }
    internal DateOnly EndDate { get; set; }

    public Quarantine() // for efc
    {
    }

    private Quarantine(DateOnly startDate)
    {
        StartDate = startDate;
        EndDate = startDate.AddDays(3);
    }
    
    public static Quarantine? CreateOrExtend(Quarantine? activeQuarantine, DateOnly startDate)
    {
        /*if (quarantines == null || quarantines.Count == 0)
        {
            return new Quarantine(startDate);
        }
        
        var latestQuarantine = quarantines.LastOrDefault(); // Get last quarantine or null*/

        if (activeQuarantine != null && activeQuarantine.EndDate >= startDate)
        {
            activeQuarantine.Extend();
            return activeQuarantine;
        }
        
        return new Quarantine(startDate);
    }

    private void Extend()
    {
        EndDate = EndDate.AddDays(3);
    }
}