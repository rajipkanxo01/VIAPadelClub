using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public class DailySchedule : AggregateRoot
{
    public Guid scheduleId;
    internal DateTime scheduleDate;
    internal TimeSpan availableFrom;
    internal TimeSpan availableUntil;
    internal ScheduleStatus status;
    internal List<Court> listOfCourts;
    internal List<Court> listOfAvailableCourts;
    internal List<Booking> listOfBookings;

    private DailySchedule()
    {
        scheduleId = Guid.NewGuid();
        scheduleDate = DateTime.Today;
        availableFrom = new TimeSpan(15, 0, 0);
        availableUntil = new TimeSpan(22, 0, 0);
        scheduleDate = DateTime.Today;
        status = ScheduleStatus.Draft;
        listOfCourts = [];
        listOfAvailableCourts = [];
        listOfBookings = [];

    }

    public static DailySchedule CreateSchedule()
    {
        return new DailySchedule();
    }
}
