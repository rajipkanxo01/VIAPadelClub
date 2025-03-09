namespace VIAPadelClub.Core.Tools.OperationResult;

public static class ErrorMessage
{
    public static Error InvalidEmailFormat() => new Error(0, "Invalid email format.");
    public static Error EmailCannotBeEmpty()=> new Error(0, "Email cannot be empty.");
    public static Error EmailMustEndWithViaDk() => new Error(0, "Email must end with @via.dk");
    public static Error InvalidFirstName() => new Error(0, "Invalid first name. Must be 2-25 letters with no symbols or spaces.");
    public static Error InvalidLastName() => new Error(0, "Invalid last name. Must be 2-25 letters with no symbols or spaces.");
    public static Error InvalidProfileUri() => new Error(0, "Invalid profile uri. Must be a valid uri.");
    public static Error InvalidStartingLetter() => new Error(0, "Invalid starting letter. Must start with 'S','s','D' or 'd'.");
    public static Error InvalidEndingNumber() => new Error(0, "Invalid ending number. Must end with number between 1-10.");
    public static Error InvalidLength() => new Error(0, "Invalid length. Must be 2 or 3 characters long.");
    public static Error ScheduleNotFound() => new Error(0, "This schedule is not found.");
    public static Error PastScheduleCannotBeUpdated() => new Error(0, "Past schedule cannot be updated.");
    public static Error InvalidScheduleStatus() => new Error(0, "Invalid schedule status. Must be Draft or Active.");
    public static Error CourtAlreadyExists()=> new Error(0, "Court already exists.");
}