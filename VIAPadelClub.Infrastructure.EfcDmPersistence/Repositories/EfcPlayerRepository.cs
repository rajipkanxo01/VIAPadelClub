using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Common;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Repositories;

public class EfPlayerRepository : RepositoryBase<Player, Email>, IPlayerRepository
{
    public EfPlayerRepository(DomainModelContext context) : base(context)
    {
        
    }
}