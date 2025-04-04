using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;

namespace UnitTests.Features.Helpers;

public class FakeTimeProvider(TimeOnly fakeCurrentTime): ITimeProvider
{
    public TimeOnly CurrentTime() => fakeCurrentTime;

}