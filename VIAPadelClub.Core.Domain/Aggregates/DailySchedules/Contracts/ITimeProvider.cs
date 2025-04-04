namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;

public interface ITimeProvider
{
    TimeOnly CurrentTime();
}
