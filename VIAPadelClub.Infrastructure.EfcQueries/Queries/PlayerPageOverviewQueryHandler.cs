using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.OperationResult;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

namespace VIAPadelClub.Infrastructure.EfcQueries.Queries;

public class PlayerPageOverviewQueryHandler(VeadatabaseProductionContext context) : IQueryHandler<PlayerPageOverview.Query, Result<PlayerPageOverview.Answer>>
{
    private readonly VeadatabaseProductionContext _context = context;

    public async Task<Result<PlayerPageOverview.Answer>> HandleAsync(PlayerPageOverview.Query query)
    {
        var player = await _context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == query.PlayerEmail);

        if (player == null)
        {
            return Result<PlayerPageOverview.Answer>.Fail(DailyScheduleError.NoPlayerFound()._message);
        }
        
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        var bookings = await _context.Bookings
            .Include(b => b.Court)
            .Where(b => b.BookedBy == query.PlayerEmail)
            .ToListAsync();

        var futureBookings = bookings
            .Where(b => DateOnly.Parse(b.BookedDate) >= now)
            .Select(b => new PlayerPageOverview.BookingSummary(
                b.BookedDate,
                b.StartTime,
                b.EndTime
            )).ToList();

        var pastBookings = bookings
            .Where(b => DateOnly.Parse(b.BookedDate) >= now)
            .Select(b => new PlayerPageOverview.BookingSummary(
                b.BookedDate,
                b.StartTime,
                b.EndTime
            )).ToList();

        var answer = new PlayerPageOverview.Answer(
            FirstName: player.FirstName,
            LastName: player.LastName,
            Email: player.Email,
            VipEndDate: player.VipendDate,
            ProfilePictureUrl: player.ProfileUrl,
            FutureBookingCount: futureBookings.Count,
            UpcomingBookings: futureBookings,
            PastBookings: pastBookings
        );
        
        return Result<PlayerPageOverview.Answer>.Ok(answer);
    }
}