using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers;

public class FakeBookingFinder: IBookingFinder
{
    private readonly List<Booking> _bookings = new(); 
    private readonly IScheduleFinder _scheduleFinder;
    
    public FakeBookingFinder(IScheduleFinder scheduleFinder)
    {
        _scheduleFinder = scheduleFinder;
    }

    public Result<IEnumerable<Booking>> FindBookingForCourt(CourtName courtName, Guid scheduleId)
    {
        var scheduleResult = _scheduleFinder.FindSchedule(scheduleId);
        if (!scheduleResult.Success)
        {
            return Result<IEnumerable<Booking>>.Fail(scheduleResult.ErrorMessage);
        }

        var schedule = scheduleResult.Data;
        var bookings = schedule.listOfbookings.Where(b => b.court.Equals(courtName));

        return Result<IEnumerable<Booking>>.Ok(bookings);
        
    }

    public void AddBooking(Booking booking)
    {
        _bookings.Add(booking);
    }
}