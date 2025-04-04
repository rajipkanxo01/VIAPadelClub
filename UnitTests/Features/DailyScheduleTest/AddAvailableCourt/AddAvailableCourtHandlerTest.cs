using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.AddAvailableCourt;

public class AddAvailableCourtHandlerTest
{
    [Fact]
    public void ShouldSucceed_WhenValidCommandIsProvided()
    {
        // Arrange
        var scheduleRepository = new FakeDailyScheduleRepository();
        var dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var scheduleFinder = new FakeScheduleFinder(scheduleRepository);
        var unitOfWork = new FakeUnitOfWork();
        
        var dailySchedule = DailyScheduleBuilder.CreateValid().Activate().BuildAsync().Data;
        var court = Court.Create(CourtName.Create("D1").Data).Data;
        
        var command = AddAvailableCourtCommand.Create(dailySchedule.scheduleId.ToString(),court.Name.Value ).Data;
        scheduleRepository.AddAsync(dailySchedule);
        
        var handler = new AddAvailableCourtHandler(scheduleRepository,dateProvider,scheduleFinder);
        
        // Act
        var result = handler.HandleAsync(command).Result;

        // Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void ShouldFail_WhenInvalidCommandIsProvided()
    {
        // Arrange
        var scheduleRepository = new FakeDailyScheduleRepository();
        var dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var scheduleFinder = new FakeScheduleFinder(scheduleRepository);
        
        var dailySchedule = DailyScheduleBuilder.CreateValid().Activate().BuildAsync().Data;
        var court = Court.Create(CourtName.Create("D1").Data).Data;
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.listOfAvailableCourts.Add(court);
        
        var command = AddAvailableCourtCommand.Create(dailySchedule.scheduleId.ToString(),court.Name.Value).Data;
        scheduleRepository.AddAsync(dailySchedule);
        
        var handler = new AddAvailableCourtHandler(scheduleRepository,dateProvider,scheduleFinder);
        
        // Act
        var result = handler.HandleAsync(command).Result;

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.CourtAlreadyExists()._message, result.ErrorMessage);
    }
}