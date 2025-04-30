using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence;

public class UnitOfWork(DomainModelContext domainModelContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => domainModelContext.SaveChangesAsync();
}