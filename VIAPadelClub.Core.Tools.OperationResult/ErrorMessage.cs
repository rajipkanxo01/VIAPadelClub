

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
    public static Error BookingStartTimeBeforeScheduleStartTime() => new Error(0, "Booking start time must be after schedule start time.");
    public static Error BookingEndTimeAfterScheduleStartTime() => new Error(0, "Booking end time must be before schedule end time.");
    public static Error BookingStartTimeAfterScheduleStartTime()=> new Error(0, "Booking start time must be before schedule end time.");
    public static Error BookingEndTimeAfterScheduleEndTime()=> new Error(0, "Booking end time must be before schedule end time.");
    public static Error QuarantinePlayerCannotBookCourt() => new Error(0, "Player is quarantined and cannot book a court.");
    public static Error PlayerIsBlacklisted() => new Error(0, "Player is blacklisted.");
    public static Error NonVipMemberCannotBookInVipTimeSlot()=> new Error(0, "Non-VIP members cannot book in VIP time slots.");
    public static Error NoPlayerFound() => new Error(0, "Player not found.");
    public static Error OneHourGapShouldBeBeforeNewBooking() => new Error(0, "There should be a one-hour gap before a new booking.");
    public static Error OneHourGapShouldBeAfterAnotherBooking() => new Error(0, "There should be a one-hour gap after another booking.");
    public static Error BookingCannotBeOverlapped() => new Error(0, "Booking cannot be overlapped.");
    public static Error OneHourGapBetweenScheduleStartTimeAndBookingStartTime() => new Error(0, "There should be a one-hour gap between the schedule start time and the booking start time.");
    public static Error OneHourGapBetweenScheduleEndTimeAndBookingEndTime() => new Error(0, "There should be a one-hour gap between the schedule end time and the booking end time.");
    public static Error ActiveCourtCannotBeRemoved() => new Error(0, "Active court cannot be removed.");
    public static Error CourtWithLaterBookingsCannotBeRemoved() => new Error(0, "Court with later bookings cannot be removed.");
      
    // Booking Errors
    public static Error ScheduleNotActive() => new Error(0, "courts cannot be booked if the Schedule is not active");
    public static Error CourtDoesntExistInSchedule() => new Error(0, "The selected court does not exist in this schedule.");
    public static Error InvalidBookingTimeSpan() => new Error(0, "The minutes of the times must be half or whole hours.");
    public static Error BookingTimeConflict() => new Error(0, "Selected time conflicts with an existing booking.");
    public static Error BookingLimitExceeded() => new Error(0, "Players can only have up to two bookings per day.");
    public static Error BookingDurationError() => new Error(0, "Booking duration must be between 1 and 3 hours.");
}