using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Common;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Repositories;

public class DailyScheduleRepository : RepositoryBase<DailySchedule, ScheduleId>, IDailyScheduleRepository
{
    private readonly DbContext _context;

    public DailyScheduleRepository(DomainModelContext context) : base(context)
    {
        _context = context;
    }


    public async Task<Result<List<DailySchedule>>> GetAllAsync()
    {
        var allSchedules = await _context.Set<DailySchedule>().ToListAsync();
        return Result<List<DailySchedule>>.Ok(allSchedules);
    }

    public override async Task<Result<DailySchedule>> GetAsync(ScheduleId id)
    {
        var dailySchedule =  await _context.Set<DailySchedule>()
            .Include(schedule => schedule.listOfCourts)
            .FirstOrDefaultAsync( schedule => schedule.ScheduleId.Equals(id));
        
        if (dailySchedule == null)
        {
            return Result<DailySchedule>.Fail(DailyScheduleError.ScheduleNotFound()._message);
        }
        
        return Result<DailySchedule>.Ok(dailySchedule);
    }
}