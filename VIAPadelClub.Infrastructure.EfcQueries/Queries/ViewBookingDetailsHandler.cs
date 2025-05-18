namespace VIAPadelClub.Infrastructure.EfcQueries.Queries;

using Core.QueryContracts.Contract;
using Core.QueryContracts.Queries;
using GeneratedModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class ViewBookingDetailsHandler : IQueryHandler<ViewBookingDetails.Query, ViewBookingDetails.Answer>
{
    private readonly VeadatabaseProductionContext _context;

    public ViewBookingDetailsHandler(VeadatabaseProductionContext context)
    {
        _context = context;
    }

    public async Task<ViewBookingDetails.Answer> HandleAsync(ViewBookingDetails.Query query)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.BookingId == query.BookingId.ToString())
            .Select(b => new ViewBookingDetails.Answer(
            Guid.Parse(b.BookingId),
            b.BookedBy,
            b.Name, // Court name
            Guid.Parse(b.ScheduleId),
            $"{b.Duration} Mins",
            b.StartTime,
            b.EndTime,
            b.BookedDate,
            b.BookingStatus
            ))
            .FirstOrDefaultAsync();

        if (booking == null)
        {
            throw new InvalidOperationException($"No booking found with ID {query.BookingId}");
        }

        return booking;
    }
}