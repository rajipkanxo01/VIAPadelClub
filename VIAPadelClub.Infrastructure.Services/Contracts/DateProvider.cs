using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;

namespace Services.Contracts;

public class DateProvider : IDateProvider
{
    public DateOnly Today()
    {
        return DateOnly.FromDateTime(DateTime.Today);
    }
}