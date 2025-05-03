using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Common;

public class EfcUnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;

    public EfcUnitOfWork(DbContext context)
    {
        _context = context;
    }

    public async Task<Result> SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error during SaveChanges: {ex.Message}");
        }
    }
}