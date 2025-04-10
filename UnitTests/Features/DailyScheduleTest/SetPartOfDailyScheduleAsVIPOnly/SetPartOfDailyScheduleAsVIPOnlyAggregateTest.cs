﻿using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.SetPartOfDailyScheduleAsVIPOnly;

using Helpers;

public class SetPartOfDailyScheduleAsVipOnlyAggregateTest
{
    [Fact] //S1
    public void Should_Set_VIP_Only_TimeSpan_When_No_PreExisting_TimeSpan()
    {
        // Arrange
        var scheduleDate = new DateOnly(2025, 03, 10);
        var startTime = new TimeOnly(10, 00);
        var endTime = new TimeOnly(14, 0);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        schedule.UpdateSchedule(scheduleDate, startTime, endTime);

        var vipStartTime = new TimeOnly(11, 00);
        var vipEndTime = new TimeOnly(13, 00);

        // Act
        var result = schedule.AddVipTimeSlots(vipStartTime, vipEndTime);

        // Assert
        Assert.True(result.Success);
        Assert.Contains((vipStartTime, vipEndTime), schedule.vipTimeRanges);
        Assert.Single(schedule.vipTimeRanges);
    }

    [Fact] //S2
    public void Should_Set_VIP_Only_TimeSpan_When_Other_TimeSpan_Exists_Results_In_Two_Separate()
    {
        // Arrange
        var scheduleDate = new DateOnly(2025, 03, 10);
        var startTime = new TimeOnly(10, 00);
        var endTime = new TimeOnly(18, 00);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        schedule.UpdateSchedule(scheduleDate, startTime, endTime);

        var firstVipStartTime = new TimeOnly(11, 00);
        var firstVipEndTime = new TimeOnly(13, 00);

        var secondVipStartTime = new TimeOnly(15, 00);
        var secondVipEndTime = new TimeOnly(17, 00);

        // Act
        var firstResult = schedule.AddVipTimeSlots(firstVipStartTime, firstVipEndTime);
        var secondResult = schedule.AddVipTimeSlots(secondVipStartTime, secondVipEndTime);

        // Assert
        Assert.True(firstResult.Success);
        Assert.True(secondResult.Success);
        Assert.Contains((firstVipStartTime, firstVipEndTime), schedule.vipTimeRanges);
        Assert.Contains((secondVipStartTime, secondVipEndTime), schedule.vipTimeRanges);
        Assert.Equal(2, schedule.vipTimeRanges.Count);
    }


    [Theory]
    [InlineData(12, 00, 14, 00, 12, 30, 14, 00, 12, 00, 14, 00)] // Start inside existing
    [InlineData(10, 00, 12, 00, 12, 00, 14, 00, 10, 00, 14, 00)] // Borders (end == start)
    [InlineData(11, 00, 13, 00, 10, 00, 11, 00, 10, 00, 13, 00)] // Borders (start == end)
    [InlineData(12, 00, 14, 00, 10, 00, 16, 00, 10, 00, 16, 00)] // Completely covers existing
    [InlineData(10, 00, 16, 00, 12, 00, 14, 00, 10, 00, 16, 00)] // Fully contained within an existing range
    [InlineData(12, 00, 16, 00, 10, 00, 14, 00, 10, 00, 16, 00)] // Partial overlap at the start
    [InlineData(10, 00, 14, 00, 12, 00, 16, 00, 10, 00, 16, 00)] // Partial overlap at the end
    public void Should_Set_VIP_Only_TimeSpan_When_Adjacent_Or_Overlapping_Results_In_Merged_TimeSpan(
        int existingStartHour, int existingStartMinute, int existingEndHour, int existingEndMinute,
        int newStartHour, int newStartMinute, int newEndHour, int newEndMinute,
        int expectedStartHour, int expectedStartMinute, int expectedEndHour, int expectedEndMinute)
    {
        // Arrange
        var scheduleDate = new DateOnly(2025, 03, 10);
        var startTime = new TimeOnly(10, 00);
        var endTime = new TimeOnly(18, 00);

        var expectedStart = new TimeOnly(expectedStartHour, expectedStartMinute);
        var expectedEnd = new TimeOnly(expectedEndHour, expectedEndMinute);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        schedule.UpdateSchedule(scheduleDate, startTime, endTime);

        var existingVipStart = new TimeOnly(existingStartHour, existingStartMinute);
        var existingVipEnd = new TimeOnly(existingEndHour, existingEndMinute);
        schedule.AddVipTimeSlots(existingVipStart, existingVipEnd);

        var newVipStart = new TimeOnly(newStartHour, newStartMinute);
        var newVipEnd = new TimeOnly(newEndHour, newEndMinute);

        // Act
        var result = schedule.AddVipTimeSlots(newVipStart, newVipEnd);

        // Assert
        Assert.True(result.Success);
        Assert.Single(schedule.vipTimeRanges);

        Assert.Contains((expectedStart, expectedEnd), schedule.vipTimeRanges);
    }


    [Fact]
    public void Should_Set_VIP_Only_TimeSpan_When_Overlapping_With_Multiple_PreExisting_TimeSpans()
    {
    }

    [Fact]
    public void Should_Fail_When_Selected_VIP_TimeSpan_Overlaps_With_NonVIP_Bookings()
    {
    }

    [Theory]
    [InlineData(9, 30, 11, 00)] // VIP start time is before schedule start time
    [InlineData(9, 30, 9, 30)] // VIP start & end times are before schedule start time
    [InlineData(12, 00, 15, 00)] // VIP end time is after schedule end time
    [InlineData(15, 00, 16, 00)] // VIP start & end times are after schedule end time
    public void Should_Fail_When_Selected_VIP_TimeSpan_Is_Outside_Of_Daily_Schedule_TimeSpan(int vipStartHour,
        int vipStartMinute, int vipEndHour, int vipEndMinute)
    {
        // Arrange
        var scheduleDate = new DateOnly(2025, 03, 10);
        var startTime = new TimeOnly(10, 00);
        var endTime = new TimeOnly(14, 00);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        schedule.UpdateSchedule(scheduleDate, startTime, endTime);

        var vipStartTime = new TimeOnly(vipStartHour, vipStartMinute);
        var vipEndTime = new TimeOnly(vipEndHour, vipEndMinute);

        // Act
        var result = schedule.AddVipTimeSlots(vipStartTime, vipEndTime);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.VipTimeOutsideSchedule()._message, result.ErrorMessage);
    }


    [Theory]
    [InlineData(11, 15, 13, 00)]
    [InlineData(11, 30, 13, 45)]
    public void Should_Fail_When_Time_Format_Is_Invalid(int vipStartHour, int vipStartMinute, int vipEndHour,
        int vipEndMinute)
    {
        // Arrange
        var scheduleDate = new DateOnly(2025, 03, 10);
        var startTime = new TimeOnly(10, 30);
        var endTime = new TimeOnly(14, 00);
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        schedule.UpdateSchedule(scheduleDate, startTime, endTime);

        var vipStartTime = new TimeOnly(11, 15);
        var vipEndTime = new TimeOnly(13, 45);

        // Act
        var result = schedule.AddVipTimeSlots(vipStartTime, vipEndTime);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(result.ErrorMessage, DailyScheduleError.InvalidTimeSlot()._message);
    }
}