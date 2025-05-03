using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Common.Repositories;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Decorator;

public class TransactionDecorator : ICommandDispatcher
{
    private readonly ICommandDispatcher _dispatcher;
    private readonly IUnitOfWork _unitOfWork;
    private bool _saveChanges;
    
    public TransactionDecorator(ICommandDispatcher dispatcher, IUnitOfWork unitOfWork)
    {
        _dispatcher = dispatcher;
        _unitOfWork = unitOfWork;
        _saveChanges = false;
    }
    
    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        var result = await _dispatcher.DispatchAsync(command);

        if (result.Success && !_saveChanges)
        {
            await _unitOfWork.SaveChangesAsync();
            _saveChanges = true;
        }

        return result;
    }
}