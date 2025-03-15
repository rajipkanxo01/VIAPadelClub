using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

namespace Services.Contracts;

public class DateProvider : IDateProvider
{
    public DateOnly Today()
    {
        return DateOnly.FromDateTime(DateTime.Today);
    }
}