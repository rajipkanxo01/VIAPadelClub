namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public interface IDateProvider
{
    DateOnly Today();
}