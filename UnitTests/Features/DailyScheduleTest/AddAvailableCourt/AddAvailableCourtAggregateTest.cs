using FluentAssertions;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.AddAvailableCourt;

public class AddAvailableCourtAggregateTest
{
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
        Assert.Contains(ErrorMessage.InvalidStartingLetter()._message, result.ErrorMessage);
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
        Assert.Contains(ErrorMessage.InvalidEndingNumber()._message, result.ErrorMessage);
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
        Assert.Contains(ErrorMessage.InvalidLength()._message, result.ErrorMessage);
    }
    
    //DailySchedule tests
    [Fact]
    public void Should_Fail_When_Schedule_Not_Found()
    {
        // Arrange
        var schedules = new List<DailySchedule>(); 
        var scheduleId = Guid.NewGuid();
        string courtName = "D1";

        // Act
        var result = new DailySchedule().AddAvailableCourt(scheduleId, courtName, schedules);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain(ErrorMessage.ScheduleNotFound()._message);
    }

    [Fact]
    public void Should_Fail_When_Schedule_Is_Past()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var schedules = new List<DailySchedule>
        {
            new DailySchedule
            {
                scheduleId = scheduleId,
                scheduleDate = DateTime.Today.AddDays(-1),
                status = ScheduleStatus.Draft,
                listOfCourts = new List<Court>()
            }
        };
        string courtName = "D1";

        // Act
        var result = new DailySchedule().AddAvailableCourt(scheduleId, courtName, schedules);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain(ErrorMessage.PastScheduleCannotBeUpdated()._message);
    }

    [Fact]
    public void Should_Fail_When_Court_Name_Is_Invalid()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var schedules = new List<DailySchedule>
        {
            new DailySchedule
            {
                scheduleId = scheduleId,
                scheduleDate = DateTime.Today,
                status = ScheduleStatus.Draft,
                listOfCourts = new List<Court>()
            }
        };
        string invalidCourtName = "F1";

        // Act
        var result = new DailySchedule().AddAvailableCourt(scheduleId, invalidCourtName, schedules);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Should_Fail_When_Court_Already_Exists()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var schedules = new List<DailySchedule>
        {
            new DailySchedule
            {
                scheduleId = scheduleId,
                scheduleDate = DateTime.Today,
                status = ScheduleStatus.Draft,
                listOfCourts = new List<Court>
                {
                    new Court(new CourtName("D1")) 
                }
            }
        };
        string courtName = "D1"; 

        // Act
        var result = new DailySchedule().AddAvailableCourt(scheduleId, courtName, schedules);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain(ErrorMessage.CourtAlreadyExists()._message);
    }

    [Fact]
    public void Should_Add_Court_Successfully_When_Valid()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var schedules = new List<DailySchedule>
        {
            new DailySchedule
            {
                scheduleId = scheduleId,
                scheduleDate = DateTime.Today,
                status = ScheduleStatus.Draft,
                listOfCourts = new List<Court>()
            }
        };
        string courtName = "D1";

        // Act
        var result = new DailySchedule().AddAvailableCourt(scheduleId, courtName, schedules);

        // Assert
        result.Success.Should().BeTrue();
        schedules[0].listOfCourts.Should().Contain(c => c.Name.Value == courtName);
    }

}