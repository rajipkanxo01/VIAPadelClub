namespace VIAPadelClub.Core.Domain.Common.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}