using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.OperationResult;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

namespace VIAPadelClub.Infrastructure.EfcQueries.Queries;

public class ViewSchedulePlayerOverviewQueryHandler(VeadatabaseProductionContext context) : IQueryHandler<ViewSchedulePlayerOverview.Query, Result<ViewSchedulePlayerOverview.Answer>>
{
    private readonly VeadatabaseProductionContext _context = context;
    
    public async Task<Result<ViewSchedulePlayerOverview.Answer>> HandleAsync(ViewSchedulePlayerOverview.Query query)
    {
        var schedule = await _context.DailySchedules
            .AsNoTracking()
            .Include(s => s.Bookings)
            .ThenInclude(c => c.Court)
            .Include(s => s.Courts)
            .Include(s => s.VipTimeRanges)
            .FirstOrDefaultAsync(s => s.ScheduleDate == query.Date && s.Status == "active");


        if (schedule == null)
        {
            return Result<ViewSchedulePlayerOverview.Answer>.Fail(DailyScheduleError.ScheduleNotFound()._message);
        }
        
        var answer = new ViewSchedulePlayerOverview.Answer(
            ScheduleId: schedule.ScheduleId,
            Date: schedule.ScheduleDate,
            Status: schedule.Status,
            AvailableCourts: schedule.Courts
                .Select(c => new ViewSchedulePlayerOverview.CourtView(c.CourtName))
                .ToList(),
            VipTimeRanges: schedule.VipTimeRanges
                .Select(v => new ViewSchedulePlayerOverview.VipTimeRangeView(v.VipStart, v.VipEnd))
                .ToList(),
            Bookings: schedule.Bookings
                .Select(b => new ViewSchedulePlayerOverview.BookingView(
                    b.BookingId,
                    b.BookedBy,
                    b.Court.CourtName,
                    b.StartTime,
                    b.EndTime,
                    b.BookingStatus.ToString()
                ))
                .ToList()
        );

        return Result<ViewSchedulePlayerOverview.Answer>.Ok(answer);
    }
}