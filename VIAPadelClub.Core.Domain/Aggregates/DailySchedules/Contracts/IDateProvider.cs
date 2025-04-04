namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;

public interface IDateProvider
{
    DateOnly Today();
}