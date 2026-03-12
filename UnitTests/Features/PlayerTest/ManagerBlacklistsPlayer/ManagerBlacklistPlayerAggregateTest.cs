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
    [Fact]
    public async Task Should_Blacklist_Player_When_Selected()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var dailySchedules = new List<DailySchedule>();

        // Act
        var result = player.Blacklist(dailySchedules);

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

        player.Quarantine(DateOnly.Parse(startDate), dailySchedules);

        // Act
        var result = player.Blacklist(dailySchedules);

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

        player.Blacklist(dailySchedules);

        // Act
        var result = player.Blacklist(dailySchedules);

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
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var playerRepo = new FakePlayerRepository();
        await playerRepo.AddAsync(player);
        var playerFinder = new FakePlayerFinder(playerRepo);

        var schedules = new List<DailySchedule>();
        var createdBookings = new List<Booking>();

        for (int i = 0; i < numberOfBookings; i++)
        {
            var court = Court.Create(CourtName.Create("S1").Data).Data;
            var dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
            var scheduleId = ScheduleId.Create();
            var scheduleRepo = new FakeDailyScheduleRepository();
            var scheduleFinder = new FakeScheduleFinder(scheduleRepo);

            var dailySchedule = DailySchedule.CreateSchedule(dateProvider, scheduleId).Data;
            dailySchedule.availableFrom = new TimeOnly(10, 0);
            dailySchedule.availableUntil = new TimeOnly(22, 0);
            dailySchedule.listOfCourts.Add(court);
            scheduleFinder.AddSchedule(dailySchedule);
            dailySchedule.Activate(dateProvider);

            var bookingResult = await dailySchedule.BookCourt(
                player.email,
                court,
                new TimeOnly(10, 0),
                new TimeOnly(11, 0),
                dateProvider,
                playerFinder,
                scheduleFinder
            );

            Assert.True(bookingResult.Success);
            createdBookings.Add(bookingResult.Data);
            schedules.Add(dailySchedule);
        }

        // Act
        var result = player.Blacklist(schedules);

        // Assert
        Assert.True(result.Success);
        Assert.True(player.isBlackListed);
        foreach (var booking in createdBookings)
        {
            Assert.Equal(BookingStatus.Cancelled, booking.BookingStatus);
        }
    }
}