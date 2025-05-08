namespace IntegrationTests.Repositories;

using Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Repositories;
using Xunit;

public class PlayerIntegrationTest
{
    [Fact]
    public async Task ShouldCreatePlayerAndSaveToDatabase_WhenValid()
    {
        // Arrange
        var (context, scheduleRepository, playerRepository) = SetupRepositories();
        var unitOfWork = new UnitOfWork(context);

        // Configure mocks
        var emailCheckerMock = new Mock<IEmailUniqueChecker>();
        emailCheckerMock.Setup(m => m.IsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));

        var dateProviderMock = new Mock<IDateProvider>();
        dateProviderMock.Setup(m => m.Today()).Returns(DateOnly.FromDateTime(DateTime.Today.AddDays(2).AddHours(2)));

        // Act - Create a player and save to database
        var email = Email.Create("325622@via.dk");
        var fullName = FullName.Create("Test", "User");
        var profileUrl = ProfileUri.Create("www.profileUri.com/testUser");

        var playerResult = await Player.Register(email.Data, fullName.Data, profileUrl.Data, emailCheckerMock.Object);
        await playerRepository.AddAsync(playerResult.Data);

        await unitOfWork.SaveChangesAsync();

        // Act - Update player to VIP status
        var storedPlayerResult = await playerRepository.GetAsync(email.Data);
        var storedPlayer = storedPlayerResult.Data;

        var vipStatusResult = storedPlayer.ChangeToVipStatus();

        // Assert - VIP status operation was successful
        Assert.True(vipStatusResult.Success);

        // Save the updated player to the database
        await unitOfWork.SaveChangesAsync();

        // Retrieve player from database for verification
        var savedPlayer = await GetPlayer(context, email.Data);

        // Assert - Player was created and saved correctly
        Assert.NotNull(savedPlayer);
        Assert.Equal(email.Data, savedPlayer.email);
        Assert.Equal(fullName.Data.FirstName, savedPlayer.fullName.FirstName);
        Assert.Equal(fullName.Data.LastName, savedPlayer.fullName.LastName);

        // Assert - VIP status was updated correctly in the database
        Assert.NotNull(savedPlayer.vipMemberShip);
        Assert.True(savedPlayer.vipMemberShip.IsVIP);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), savedPlayer.vipMemberShip.VIPStartDate);
    }

    /// <summary>
    /// Retrieves a player from the database by email
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="email">Email of the player to retrieve</param>
    /// <returns>Player entity</returns>
    private static async Task<Player> GetPlayer(DomainModelContext context, Email email)
    {
        var player = await context.Set<Player>()
            .AsNoTracking()
            .FirstAsync(player => player.email == email);

        return player;
    }

    /// <summary>
    /// Sets up the repositories and database context for testing
    /// </summary>
    /// <returns>Tuple containing context and repositories</returns>
    private static (DomainModelContext context, IDailyScheduleRepository scheduleRepository, IPlayerRepository
        playerRepository) SetupRepositories()
    {
        var context = MyDbContext.SetupContext();
        return (
            context,
            new DailyScheduleRepository(context),
            new PlayerRepository(context)
        );
    }
}