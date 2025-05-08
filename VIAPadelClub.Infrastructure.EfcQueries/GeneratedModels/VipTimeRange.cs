using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

public partial class VipTimeRange
{
    public string VipStart { get; set; } = null!;

    public string VipEnd { get; set; } = null!;

    public string DailyScheduleId { get; set; } = null!;

    public virtual DailySchedule DailySchedule { get; set; } = null!;
}
