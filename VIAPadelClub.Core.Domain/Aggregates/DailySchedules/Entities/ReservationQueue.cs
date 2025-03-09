using VIAPadelClub.Core.Domain.Aggregates.Players;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

public class ReservationQueue
{
    internal Guid QueueId { get;}
    internal Guid ScheduleId { get;}
    internal List<Player> PlayersInQueue { get; }
    
    
    
}