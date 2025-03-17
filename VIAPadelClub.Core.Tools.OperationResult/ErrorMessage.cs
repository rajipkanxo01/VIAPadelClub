

namespace VIAPadelClub.Core.Tools.OperationResult;

public static class ErrorMessage
{
    // Player Error Message
    public static Error InvalidEmailFormat() => new Error(0, "Invalid email format.");
    public static Error EmailCannotBeEmpty()=> new Error(0, "Email cannot be empty.");
    public static Error EmailMustEndWithViaDk() => new Error(0, "Email must end with @via.dk");
    public static Error InvalidFirstName() => new Error(0, "Invalid first name. Must be 2-25 letters with no symbols or spaces.");
    public static Error InvalidLastName() => new Error(0, "Invalid last name. Must be 2-25 letters with no symbols or spaces.");
    public static Error InvalidProfileUri() => new Error(0, "Invalid profile uri. Must be a valid uri.");
    public static Error BlackListedCannotQuarantine() => new Error(0, "Player is already blacklisted and cannot be quarantined.");
    
    // Daily Schedule Error Message
    public static Error InvalidStartingLetter() => new Error(0, "Invalid starting letter. Must start with 'S','s','D' or 'd'.");
    public static Error InvalidEndingNumber() => new Error(0, "Invalid ending number. Must end with number between 1-10.");
    public static Error InvalidLength() => new Error(0, "Invalid length. Must be 2 or 3 characters long.");
    public static Error ScheduleNotFound() => new Error(0, "This schedule is not found.");
    public static Error PastScheduleCannotBeUpdated() => new Error(0, "Past schedule cannot be updated.");
    public static Error InvalidScheduleStatus() => new Error(0, "Invalid schedule status. Must be Draft or Active.");
    public static Error CourtAlreadyExists()=> new Error(0, "Court already exists.");
    public static Error InvalidTimeSlot() => new Error(0, "VIP time spans must start and end at whole or half-hour marks (e.g., 10:00, 10:30, 11:00, etc.).");
    public static Error VipTimeOutsideSchedule() => new Error(0, "VIP time must be within daily schedule time.");
    public static Error NoCourtAvailable() => new Error(0, "No court available.");
    public static Error PastScheduleCannotBeActivated() => new Error(0, "Past schedule cannot be activated.");
    public static Error ScheduleAlreadyActive()=> new Error(0, "Schedule is already active.");
    public static Error ScheduleIsDeleted() => new Error(0, "Schedule is deleted.");
    public static Error DuplicateEmail() => new Error(0, "Email already exists.");
    public static Error ScheduleAlreadyDeleted() => new Error(0, "Schedule is already deleted.");
    public static Error PastScheduleCannotBeDeleted() => new Error(0, "Cannot delete a schedule from the past.");
    public static Error SameDayActiveScheduleCannotBeDeleted() => new Error(0, "Cannot delete an active schedule on the same day.");
    public static Error ScheduleCannotBeUpdatedWithPastDate() => new Error(0, "Cannot update schedule with past date.");
    public static Error ScheduleEndDateMustBeAfterStartDate() => new Error(0, "The end time must be after the start time.");
    public static Error ScheduleInvalidTimeSpan() => new Error(0, "The time interval must span 60 minutes or more.");
    public static Error InvalidScheduleUpdateStatus() => new Error(0, "An active daily schedule cannot be modified, only deleted.");
    public static Error InvalidScheduleTimeSpan() => new Error(0, "The minutes of the times must be half or whole hours.");
}