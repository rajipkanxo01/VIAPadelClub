using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers;

public class FakeUnitOfWork : IUnitOfWork
{
    public bool SaveChangesCalled { get; private set; }

    public Task<Result> SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.FromResult(Result.Ok());
    }
}