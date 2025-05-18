using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

public partial class DailySchedule
{
    public string ScheduleId { get; set; } = null!;

    public string AvailableFrom { get; set; } = null!;

    public string AvailableUntil { get; set; } = null!;

    public int IsDeleted { get; set; }

    public string ScheduleDate { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Court> Courts { get; set; } = new List<Court>();

    public virtual ICollection<VipTimeRange> VipTimeRanges { get; set; } = new List<VipTimeRange>();
}
