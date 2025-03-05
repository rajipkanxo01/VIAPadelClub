using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players.Values;

public class ProfileUri
{
    internal string Value { get; }
    
    private ProfileUri(string value)
    {
        Value = value;
    }
    
    public static Result<ProfileUri> Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Result<ProfileUri>.Fail(ErrorMessage.InvalidProfileUri()._message);
        }
        return Result<ProfileUri>.Ok(new ProfileUri(url));
    }
}