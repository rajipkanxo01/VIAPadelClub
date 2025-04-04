namespace VIAPadelClub.Core.Tools.OperationResult;

public static class PlayerError
{
    // Player Error Message
    public static Error InvalidEmailFormat() => new Error(0, "Invalid email format.");
    public static Error EmailCannotBeEmpty()=> new Error(0, "Email cannot be empty.");
    public static Error EmailMustEndWithViaDk() => new Error(0, "Email must end with @via.dk");
    public static Error InvalidFirstName() => new Error(0, "Invalid first name. Must be 2-25 letters with no symbols or spaces.");
    public static Error InvalidLastName() => new Error(0, "Invalid last name. Must be 2-25 letters with no symbols or spaces.");
    public static Error InvalidProfileUri() => new Error(0, "Invalid profile uri. Must be a valid uri.");
    public static Error BlackListedCannotQuarantine() => new Error(0, "Player is already blacklisted and cannot be quarantined.");
    
}