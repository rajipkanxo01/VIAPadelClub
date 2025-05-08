using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Common;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Repositories;

public class PlayerRepository : RepositoryBase<Player, Email>, IPlayerRepository
{
    public PlayerRepository(DomainModelContext context) : base(context)
    {
        
    }
}