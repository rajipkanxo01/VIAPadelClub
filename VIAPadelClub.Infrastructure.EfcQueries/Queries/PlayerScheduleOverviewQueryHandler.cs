using System.Globalization;
using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.OperationResult;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

namespace VIAPadelClub.Infrastructure.EfcQueries.Queries;

public class PlayerScheduleOverviewQueryHandler(VeadatabaseProductionContext context) : IQueryHandler<PlayerScheduleOverview.Query, Result<PlayerScheduleOverview.Answer>>
{
    private readonly VeadatabaseProductionContext _context = context;

    public async Task<Result<PlayerScheduleOverview.Answer>> HandleAsync(PlayerScheduleOverview.Query query)
    {
        if (!DateTime.TryParseExact(query.MonthName, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            return Result<PlayerScheduleOverview.Answer>.Fail($"Invalid month name: {query.MonthName}");
        }

        int month = parsedDate.Month;

        var allSchedules = await _context.DailySchedules
            .AsNoTracking()
            .Include(d => d.Courts)
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
            .Select(s =>
            {
                DateTime.TryParse(s.ScheduleDate, out var date);
                var scheduleStatus = date < DateTime.Today ? "Closed" : s.Status;

                return new PlayerScheduleOverview.ScheduleView(
                    s.ScheduleId.ToString(),
                    s.ScheduleDate,
                    scheduleStatus
                );
            })
            .ToList();

        var answer = new PlayerScheduleOverview.Answer(schedules);
        return Result<PlayerScheduleOverview.Answer>.Ok(answer);
    }

}