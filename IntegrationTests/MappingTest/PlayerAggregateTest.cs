using IntegrationTests.Helpers;
using Services.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using Xunit;

namespace IntegrationTests.MappingTest;

[Collection("Sequential")]
public class PlayerAggregateTest
{
    //NonNullableSinglePrimitiveValuedValueObject
    [Fact]
    public async Task ShouldSucceed_WhenEmailIsNotNull_AndIsPersisted()
    {
        await using MyDbContext ctx = MyDbContext.SetupContext();

        // Arrange
        var email = Email.Create("via@via.dk").Data;
        var fullName = FullName.Create("first", "second").Data;
        var profileUri = ProfileUri.Create("http://example.com").Data;
        var emailUniqueChecker = new EmailUniqueChecker();

        var result = await Player.Register(email, fullName, profileUri, emailUniqueChecker);

        var player = result.Data;     

        await MyDbContext.SaveAndClearAsync(player, ctx);

        // Act
        var retrieved = ctx.Set<Player>()
            .Single(p => p.email.Equals(email));

        // Assert
        Assert.NotNull(retrieved.email);
        Assert.Equal(email.Value, retrieved.email.Value);
    }
    
    [Fact]
    public async Task ShouldSucceed_WhenFullNameIsNotNull_AndIsPersisted()
    {
        await using MyDbContext ctx = MyDbContext.SetupContext();

        // Arrange
        var email = Email.Create("viaa@via.dk").Data;
        var fullName = FullName.Create("first", "second").Data;
        var profileUri = ProfileUri.Create("http://example.com").Data;
        var emailUniqueChecker = new EmailUniqueChecker();

        var result = await Player.Register(email, fullName, profileUri, emailUniqueChecker);

        var player = result.Data;     

        await MyDbContext.SaveAndClearAsync(player, ctx);

        // Act
        var retrieved = ctx.Set<Player>()
            .Single(p => p.email.Equals(email));

        // Assert
        Assert.NotNull(retrieved.fullName.FirstName);
        Assert.Equal(fullName.FirstName, retrieved.fullName.FirstName);
    }
    
    [Fact]
    public async Task ShouldSucceed_WhenProfileUrlIsNotNull_AndIsPersisted()
    {
        await using MyDbContext ctx = MyDbContext.SetupContext();

        // Arrange
        var email = Email.Create("vias@via.dk").Data;
        var fullName = FullName.Create("first", "second").Data;
        var profileUri = ProfileUri.Create("http://example.com").Data;
        var emailUniqueChecker = new EmailUniqueChecker();

        var result = await Player.Register(email, fullName, profileUri, emailUniqueChecker);

        var player = result.Data;     

        await MyDbContext.SaveAndClearAsync(player, ctx);

        // Act
        var retrieved = ctx.Set<Player>()
            .Single(p => p.email.Equals(email));

        // Assert
        Assert.NotNull(retrieved.url);
        Assert.Equal(profileUri.Value, retrieved.url.Value);
    }


}