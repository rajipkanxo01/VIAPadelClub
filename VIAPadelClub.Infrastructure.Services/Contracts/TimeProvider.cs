using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;

namespace Services.Contracts;

public class TimeProvider : ITimeProvider
{
    public TimeOnly CurrentTime()
    {
        return TimeOnly.FromDateTime(DateTime.Today);
    }
}