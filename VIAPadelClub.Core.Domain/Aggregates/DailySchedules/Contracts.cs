using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public interface IDateProvider
{
    DateOnly Today();
}

public interface IScheduleFinder
{
    Result<DailySchedule> FindSchedule(Guid scheduleId);
    void AddSchedule(DailySchedule schedule);    //Todo: Remove Add schedule after session 6
}

public interface IBookingFinder
{
    Result<IEnumerable<Booking>> FindBookingForCourt(CourtName courtName,Guid scheduleId);
    void AddBooking(Booking booking);    //Todo: Remove Add booking after session 6
}