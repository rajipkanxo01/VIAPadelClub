using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

namespace UnitTests.Features.Helpers;

public class FakeDateProvider(DateOnly fakeDate) : IDateProvider
{
    public DateOnly Today() => fakeDate;
}