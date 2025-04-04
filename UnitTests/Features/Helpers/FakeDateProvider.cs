using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;

namespace UnitTests.Features.Helpers;

public class FakeDateProvider(DateOnly fakeDate) : IDateProvider
{
    public DateOnly Today() => fakeDate;
}