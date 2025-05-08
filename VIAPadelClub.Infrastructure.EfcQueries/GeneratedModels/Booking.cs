using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public string BookedBy { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ScheduleId { get; set; } = null!;

    public int Duration { get; set; }

    public string StartTime { get; set; } = null!;

    public string EndTime { get; set; } = null!;

    public string BookedDate { get; set; } = null!;

    public string BookingStatus { get; set; } = null!;

    public virtual Player BookedByNavigation { get; set; } = null!;

    public virtual Court Court { get; set; } = null!;

    public virtual DailySchedule Schedule { get; set; } = null!;
}
