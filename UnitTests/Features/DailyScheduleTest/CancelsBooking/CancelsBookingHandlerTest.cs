using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CancelsBooking;

public class CancelsBookingHandlerTest
{
    [Fact]
    public void ShouldCancelBooking_WhenCommandIsValid()
    {
        // Arrange
        var fakeDailyScheduleRepository = new FakeDailyScheduleRepository();
        var fakePlayerRepository = new FakePlayerRepository();
        
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var fakePlayerFinder = new FakePlayerFinder(fakePlayerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(fakeDailyScheduleRepository);
        var fakeTimeProvider = new FakeTimeProvider(new TimeOnly(10, 0, 0));
        
        var court = Court.Create(CourtName.Create("D1").Data).Data;

        var player = PlayerBuilder.CreateValid().BuildAsync().Result.Data;
        var dailySchedule = DailyScheduleBuilder.CreateValid().WithDateProvider(fakeDateProvider).Activate().WithCourt(court)
            .WithScheduleFinder(fakeScheduleFinder).BuildAsync().Data;
        var scheduleId = ScheduleId.FromGuid(dailySchedule.ScheduleId.Value);

        fakeDailyScheduleRepository.AddAsync(dailySchedule);
        fakePlayerRepository.AddAsync(player);


        var bookingStartTime = new TimeOnly(15, 0, 0);
        var bookingEndTime = new TimeOnly(17, 0, 0);

        var booking = dailySchedule.BookCourt(player.email, court, bookingStartTime, bookingEndTime, fakeDateProvider,
            fakePlayerFinder, fakeScheduleFinder).Data;
        
        var cancelsBookingCommand = PlayerCancelsBookingCommand
            .Create(booking.BookingId.ToString(), player.email.Value, scheduleId.Value.ToString()).Data;

        var handler = new PlayerCancelsBookingHandler(fakeDailyScheduleRepository, fakeDateProvider, fakeTimeProvider);

        // Act
        var result = handler.HandleAsync(cancelsBookingCommand).Result;

        // Assert
        Assert.True(result.Success);
        Assert.Single(dailySchedule.listOfBookings);
    }

    [Fact]
    public void ShouldFail_WhenBookingDoesNotExist()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var fakeDailyScheduleRepository = new FakeDailyScheduleRepository();
        var fakePlayerRepository = new FakePlayerRepository();
        var fakePlayerFinder = new FakePlayerFinder(fakePlayerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(fakeDailyScheduleRepository);
        var fakeTimeProvider = new FakeTimeProvider(new TimeOnly(10, 0, 0));

        var player = PlayerBuilder.CreateValid().BuildAsync().Result.Data;
        var dailySchedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;
        var scheduleId = ScheduleId.FromGuid(dailySchedule.ScheduleId.Value);

        var bookingStartTime = new TimeOnly(15, 0, 0);
        var bookingEndTime = new TimeOnly(17, 0, 0);

        var court = Court.Create(CourtName.Create("D1").Data).Data;

        var booking = dailySchedule.BookCourt(player.email, court, bookingStartTime, bookingEndTime, fakeDateProvider,
            fakePlayerFinder, fakeScheduleFinder).Data;

        fakeDailyScheduleRepository.AddAsync(dailySchedule);
        fakePlayerRepository.AddAsync(player);

        var cancelsBookingCommand = PlayerCancelsBookingCommand
            .Create(Guid.NewGuid().ToString(), player.email.Value, scheduleId.Value.ToString()).Data;

        var handler = new PlayerCancelsBookingHandler(fakeDailyScheduleRepository, fakeDateProvider, fakeTimeProvider);

        // Act
        var result = handler.HandleAsync(cancelsBookingCommand).Result;

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingNotFound()._message, result.ErrorMessage);
    }
}