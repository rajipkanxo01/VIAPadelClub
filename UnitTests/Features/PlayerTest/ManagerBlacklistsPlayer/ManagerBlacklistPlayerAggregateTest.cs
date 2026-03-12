using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerBlacklistsPlayer;

public class ManagerBlacklistPlayerAggregateTest
{
    private readonly FakeDailyScheduleRepository dailyScheduleRepository = new();
    private readonly FakePlayerRepository playerRepository = new();
    [Fact]
    public async Task Should_Blacklist_Player_When_Selected()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var dailySchedules = new List<DailySchedule>();

        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        // Act
        var result = await player.Blacklist(fakeScheduleFinder);

        // Assert
        Assert.True(player.isBlackListed);
        Assert.True(result.Success);
    }

    [Theory]
    [InlineData("2025-03-15")]
    [InlineData("2025-04-22")]
    [InlineData("2025-04-26")]
    public async Task Should_Remove_Quarantine_When_Player_Is_Blacklisted(string startDate)
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var dailySchedules = new List<DailySchedule>();

        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        player.Quarantine(DateOnly.Parse(startDate), dailySchedules);

        // Act
        var result = await player.Blacklist(fakeScheduleFinder);

        // Assert
        Assert.True(result.Success);
        Assert.True(player.isBlackListed);
        Assert.Null(player.activeQuarantine);
    }

    [Fact]
    public async Task Should_Fail_If_Player_Is_Already_Blacklisted()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        
        var dailySchedules = new List<DailySchedule>();
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);
        
        await player.Blacklist(fakeScheduleFinder);

        // Act
        var result = await player.Blacklist(fakeScheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.PlayerAlreadyBlacklisted()._message, result.ErrorMessage);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(0)]
    public async Task Should_Cancel_Booked_Courts_When_Player_Is_Blacklisted(int numberOfBookings)
    {
        // Arrange
        var scheduleRepo = new FakeDailyScheduleRepository();
        var fakeScheduleFinder = new FakeScheduleFinder(scheduleRepo);
        var fakePlayerRepo = new FakePlayerRepository();
        var fakePlayerFinder = new FakePlayerFinder(fakePlayerRepo);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        await fakePlayerRepo.AddAsync(player);

        var bookings = new List<Booking>();

        for (int i = 0; i < numberOfBookings; i++)
        {
            var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
            var court = Court.Create(CourtName.Create("S1").Data).Data;

            var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;
            schedule.availableFrom = new TimeOnly(9, 0);
            schedule.availableUntil = new TimeOnly(17, 0);
            schedule.listOfCourts.Add(court);
            schedule.Activate(fakeDateProvider);

            await scheduleRepo.AddAsync(schedule);
            fakeScheduleFinder.AddSchedule(schedule);

            var booking = await schedule.BookCourt(
                player.email, court,
                new TimeOnly(10, 0), new TimeOnly(11, 0),
                fakeDateProvider, fakePlayerFinder, fakeScheduleFinder);

            schedule.listOfBookings.Add(booking.Data);
            bookings.Add(booking.Data);
        }

        // Act
        var result = await player.Blacklist(fakeScheduleFinder);

        // Assert
        Assert.True(result.Success);
        Assert.True(player.isBlackListed);
        Assert.All(bookings, b => Assert.Equal(BookingStatus.Cancelled, b.BookingStatus));
    }
    
}
