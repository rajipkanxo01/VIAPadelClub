using FluentAssertions;
using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.AddAvailableCourt;

public class AddAvailableCourtAggregateTest
{
    private readonly FakeDailyScheduleRepository dailyScheduleRepository = new FakeDailyScheduleRepository();
    private readonly FakePlayerRepository playerRepository = new FakePlayerRepository();
    
    //CourtName tests
    [Theory]
    [InlineData("A1")]
    [InlineData("s")]
    [InlineData("D123")] 
    public void Should_Failed_When_Name_Is_InValid(string name)
    {
        // Act
        var result = CourtName.Create(name);

        // Assert
        result.Success.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("S1")]
    [InlineData("D1")]
    public void Should_Succeed_When_Name_Is_Valid(string name)
    {
        // Act
        var result = CourtName.Create(name);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Value.Should().Be(name);
    }
    
    [Theory]
    [InlineData("S1")]
    [InlineData("D1")]
    public void Should_Succeed_When_Starting_Letter_Is_Valid(string name)
    {
        // Act
        var result = CourtName.Create(name);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Value.Should().Be(name);
    }
    
    [Theory]
    [InlineData("a1")]
    [InlineData("Z1")]
    public void Should_Fail_When_Starting_Letter_Is_InValid(string name)
    {
        // Act
        var result = CourtName.Create(name);

        // Assert
        result.Success.Should().BeFalse();
        Assert.NotEmpty(result.ErrorMessage);
        Assert.Contains(DailyScheduleError.InvalidStartingLetter()._message, result.ErrorMessage);
    }
    
    [Theory]
    [InlineData("S1")]
    [InlineData("D1")]
    public void Should_Succeed_When_Ending_Number_Is_Valid(string name)
    {
        // Act
        var result = CourtName.Create(name);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Value.Should().Be(name);
    }
    
    [Theory]
    [InlineData("a11")]
    [InlineData("Z98")]
    public void Should_Fail_When_Ending_Number_Is_InValid(string name)
    {
        // Act
        var result = CourtName.Create(name);

        // Assert
        result.Success.Should().BeFalse();
        Assert.Contains(DailyScheduleError.InvalidEndingNumber()._message, result.ErrorMessage);
    }

    [Theory]
    [InlineData("S1")]
    [InlineData("D1")]
    public void Should_Succeed_When_Length_Is_Valid(string name)
    {
        // Act
        var result = CourtName.Create(name);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Value.Should().Be(name);
    }
    
    [Theory]
    [InlineData("S11231")]
    [InlineData("D123")]
    public void Should_Fail_When_Length_Is_InValid(string name)
    {
        // Act
        var result = CourtName.Create(name);

        // Assert
        result.Success.Should().BeFalse();
        Assert.Contains(DailyScheduleError.InvalidLength()._message, result.ErrorMessage);
    }
    
    //Daily_Schedule tests
    [Fact]
     public void Should_Fail_When_Schedule_Not_Found()
     {
         // Arrange
         var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

         var fakeScheduleRepo= new FakeScheduleFinder(dailyScheduleRepository);
         var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
         var scheduleResult = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId);
         string courtName = "D1";

         // Act
         var result = scheduleResult.Data.AddAvailableCourt(Court.Create(CourtName.Create(courtName).Data).Data, fakeDateProvider, fakeScheduleRepo);
    
         // Assert
         result.Success.Should().BeFalse();
         result.ErrorMessage.Should().Contain(DailyScheduleError.ScheduleNotFound()._message);
     }

    [Fact]
    public void Should_Fail_When_Schedule_Is_Past()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var fakeScheduleRepo= new FakeScheduleFinder(dailyScheduleRepository);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)); 
        
        var scheduleResult = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId);
        scheduleResult.Data.scheduleDate = fakeDateProvider.Today().AddDays(-1);
        fakeScheduleRepo.AddSchedule(scheduleResult.Data);
        string courtName = "D1";
    
        // Act
        var result = scheduleResult.Data.AddAvailableCourt(Court.Create(CourtName.Create(courtName).Data).Data, fakeDateProvider, fakeScheduleRepo);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain(DailyScheduleError.PastScheduleCannotBeUpdated()._message);
    }
    
    [Fact]
    public void Should_Fail_When_Court_Name_Is_Invalid()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var fakeScheduleRepo= new FakeScheduleFinder(dailyScheduleRepository);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var scheduleResult = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId);
        string invalidCourtName = "F1";
        
        // Act
        var result = scheduleResult.Data.AddAvailableCourt(Court.Create(CourtName.Create(invalidCourtName).Data).Data, fakeDateProvider, fakeScheduleRepo);

        // Assert
        result.Success.Should().BeFalse();
    }
    
    [Fact]
    public void Should_Fail_When_Court_Already_Exists_In_Daily_Schedule()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var fakeScheduleRepo= new FakeScheduleFinder(dailyScheduleRepository);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var scheduleResult = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId);
        fakeScheduleRepo.AddSchedule(scheduleResult.Data);
        string courtName = "D1";

        scheduleResult.Data.AddAvailableCourt(Court.Create(CourtName.Create(courtName).Data).Data, fakeDateProvider, fakeScheduleRepo);
    
        // Act
        var result = scheduleResult.Data.AddAvailableCourt(Court.Create(CourtName.Create(courtName).Data).Data, fakeDateProvider, fakeScheduleRepo);
    
        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain(DailyScheduleError.CourtAlreadyExists()._message);
    }
    
    [Fact]
    public void Should_Add_Court_Successfully_When_Valid()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
        
        var fakeScheduleRepo= new FakeScheduleFinder(dailyScheduleRepository);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var scheduleResult = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId);
        fakeScheduleRepo.AddSchedule(scheduleResult.Data);
        string courtName = "D1";
        
        // Act
        var result = scheduleResult.Data.AddAvailableCourt(Court.Create(CourtName.Create(courtName).Data).Data, fakeDateProvider, fakeScheduleRepo);
    
        // Assert
        result.Success.Should().BeTrue();
        scheduleResult.Data.listOfCourts.Should().Contain(c => c.Name.Value == courtName);
    }
    
    
}

