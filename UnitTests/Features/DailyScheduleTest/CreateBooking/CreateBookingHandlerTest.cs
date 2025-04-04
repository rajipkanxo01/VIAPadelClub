using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CreateBooking;

public class CreateBookingHandlerTest
{
    [Fact]
    public void Should_Create_Booking_When_Valid_Input_Is_Provided()
    {
        // Arrange
        var fakeDailyScheduleRepository = new FakeDailyScheduleRepository();
        var fakePlayerRepository = new FakePlayerRepository();

        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var fakePlayerFinder = new FakePlayerFinder(fakePlayerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(fakeDailyScheduleRepository);
        var unitOfWork = new FakeUnitOfWork();

        var court = Court.Create(CourtName.Create("D1").Data).Data;

        var player = PlayerBuilder.CreateValid().BuildAsync().Result.Data;
        var dailySchedule = DailyScheduleBuilder.CreateValid().WithDateProvider(fakeDateProvider).Activate()
            .WithCourt(court)
            .WithScheduleFinder(fakeScheduleFinder).BuildAsync().Data;
        
        fakeDailyScheduleRepository.AddAsync(dailySchedule);
        fakePlayerRepository.AddAsync(player);
        
        var bookingStartTime = new TimeOnly(15, 0, 0);
        var bookingEndTime = new TimeOnly(16, 0, 0);

        var createBookingCommand = CreateBookingCommand.Create(dailySchedule.scheduleId.ToString(), player.email.Value,
            bookingStartTime.ToString(), bookingEndTime.ToString(), court.Name.Value).Data;
        var handler = new CreateBookingHandler(fakeDailyScheduleRepository, unitOfWork, fakeDateProvider,
            fakePlayerFinder, fakeScheduleFinder);

        //Act
        var result = handler.HandleAsync(createBookingCommand).Result;

        //Assert
        Assert.True(result.Success);
    }
}