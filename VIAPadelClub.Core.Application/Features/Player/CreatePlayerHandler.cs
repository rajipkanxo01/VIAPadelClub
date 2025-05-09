﻿using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Booking;

public class CreatePlayerHandler: ICommandHandler<CreatePlayerCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IEmailUniqueChecker _emailUniqueChecker;
    
    public CreatePlayerHandler(IPlayerRepository playerRepository, IEmailUniqueChecker emailUniqueChecker)
    {
        _playerRepository = playerRepository;
        _emailUniqueChecker = emailUniqueChecker;
    }


    public Task<Result> HandleAsync(CreatePlayerCommand command)
    {
        var playerResult = Player.Register(command.Email, command.FullName, command.ProfileUri, _emailUniqueChecker);
        
        if (!playerResult.Result.Success)
        {
            return Task.FromResult(Result.Fail(playerResult.Result.ErrorMessage));
        }
        
        _playerRepository.AddAsync(playerResult.Result.Data);
        
        return Task.FromResult(Result.Ok());
    }
}