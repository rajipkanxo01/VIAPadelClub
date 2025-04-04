using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players.Values;

public class FullName
{
    internal string FirstName { get; }
    internal string LastName { get; }
    
    private FullName(string firstName, string lastName)
    {
        FirstName = char.ToUpper(firstName[0]) + firstName.Substring(1).ToLower();
        LastName = char.ToUpper(lastName[0]) + lastName.Substring(1).ToLower();
    }
    
    public static Result<FullName> Create(string firstName, string lastName)
    {   
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 25 || !firstName.All(char.IsLetter))
        {
            return Result<FullName>.Fail(PlayerError.InvalidFirstName()._message);
        }
        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 25 || !lastName.All(char.IsLetter))
        {
            return Result<FullName>.Fail(PlayerError.InvalidLastName()._message);
        }
        return Result<FullName>.Ok(new FullName(firstName, lastName));
    }
}