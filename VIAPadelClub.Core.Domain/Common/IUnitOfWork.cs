using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}