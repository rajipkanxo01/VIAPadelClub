using System;
using System.Collections.Generic;

namespace VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

public partial class Player
{
    public string Email { get; set; } = null!;

    public int IsBlackListed { get; set; }

    public int IsQuarantined { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ProfileUrl { get; set; } = null!;

    public string? QuarantineStartDate { get; set; }

    public string? QuarantineEndDate { get; set; }

    public string? VipstartDate { get; set; }

    public string? VipendDate { get; set; }

    public int? IsVip { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
