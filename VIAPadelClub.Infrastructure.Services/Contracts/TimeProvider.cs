﻿using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

namespace Services.Contracts;

public class TimeProvider : ITimeProvider
{
    public TimeOnly CurrentTime()
    {
        return TimeOnly.FromDateTime(DateTime.Today);
    }
}