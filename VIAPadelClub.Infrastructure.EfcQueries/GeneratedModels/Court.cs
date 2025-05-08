using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

public partial class Court
{
    public string CourtName { get; set; } = null!;

    public string ScheduleId { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual DailySchedule Schedule { get; set; } = null!;
}
