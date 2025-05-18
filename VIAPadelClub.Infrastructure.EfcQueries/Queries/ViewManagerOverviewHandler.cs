using System.Globalization;
using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.OperationResult;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

namespace VIAPadelClub.Infrastructure.EfcQueries.Queries;

public class ViewManagerOverviewHandler(VeadatabaseProductionContext context)
    : IQueryHandler<ViewManagerOverview.Query, Result<ViewManagerOverview.Answer>>
{
    private readonly VeadatabaseProductionContext _context = context;
    
    public async Task<Result<ViewManagerOverview.Answer>> HandleAsync(ViewManagerOverview.Query query)
    {
        if (!DateTime.TryParseExact(query.MonthName, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            return Result<ViewManagerOverview.Answer>.Fail($"Invalid month name: {query.MonthName}");
        }

        int month = parsedDate.Month;

        var allSchedules = await _context.DailySchedules
            .AsNoTracking().Include(dailySchedule => dailySchedule.Courts)
            .ToListAsync();

        var schedules = allSchedules
            .Where(s =>
            {
                if (DateTime.TryParse(s.ScheduleDate, out var date))
                {
                    return date.Month == month;
                }
                return false;
            })
            .Select(s => new ViewManagerOverview.ScheduleView(
                s.ScheduleId.ToString(),
                s.ScheduleDate,
                s.Status,
                s.Courts.Count
            ))
            .ToList();


        var answer = new ViewManagerOverview.Answer(schedules);
        return Result<ViewManagerOverview.Answer>.Ok(answer);
    }

}