using IntegrationTests.Helpers;
using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using Xunit;
using Assert = Xunit.Assert;

namespace IntegrationTests;

public class DbContext
{
    [Fact]
    public async Task StrongIdAsPk()
    {
        await using MyDbContext ctx = MyDbContext.SetupContext();

        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var id = Guid.NewGuid();

        ScheduleId scheduleId = ScheduleId.FromGuid(id);
        DailySchedule entity = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;

        await MyDbContext.SaveAndClearAsync(entity, ctx);

        DailySchedule? retrieved = ctx.DailySchedules.SingleOrDefault(x => x.scheduleId.Equals(scheduleId));
        Assert.NotNull(retrieved);
    }

    [Fact]
    public async Task NonNullableSinglePrimitiveValuedValueObject()
    {
        
    }
}