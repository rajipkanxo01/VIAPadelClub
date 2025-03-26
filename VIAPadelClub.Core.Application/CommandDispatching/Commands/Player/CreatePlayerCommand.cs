using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;

public class CreatePlayerCommand
{
    internal Email Email { get; private set; }
    internal FullName FullName { get; private set; }
    internal ProfileUri ProfileUri { get; private set; }
    
    private CreatePlayerCommand(Email email, FullName fullName, ProfileUri profileUri)
    {
        Email = email;
        FullName = fullName;
        ProfileUri = profileUri;
    }
    
    public static Result<CreatePlayerCommand> Create(string email, string firstName,string lastName, string profileUri)
    {
        var emailResult = Email.Create(email);
        var fullNameResult = FullName.Create(firstName, lastName);
        var profileUriResult = ProfileUri.Create(profileUri);

        if (!emailResult.Success)
        {
            return Result<CreatePlayerCommand>.Fail(emailResult.ErrorMessage);
        }
        if (!fullNameResult.Success)
        {
            return Result<CreatePlayerCommand>.Fail(fullNameResult.ErrorMessage);
        }
        if (!profileUriResult.Success)
        {
            return Result<CreatePlayerCommand>.Fail(profileUriResult.ErrorMessage);
        }

        var command = new CreatePlayerCommand(emailResult.Data, fullNameResult.Data, profileUriResult.Data);
        return Result<CreatePlayerCommand>.Ok(command);
    }
}