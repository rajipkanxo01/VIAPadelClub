using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Common.Repositories;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence;

public class UnitOfWork(DomainModelContext domainModelContext) : IUnitOfWork
{
    public Task SaveChangesAsync() => domainModelContext.SaveChangesAsync();
}