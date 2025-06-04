using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

namespace IntegrationTests.Helpers;

internal class PadelClubWebApplicationFactory : WebApplicationFactory<Program>
{
    private IServiceCollection _serviceCollection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            _serviceCollection = services;

            services.RemoveAll(typeof(DbContextOptions<DomainModelContext>));
            services.RemoveAll(typeof(DbContextOptions<VeadatabaseProductionContext>));
            services.RemoveAll<DomainModelContext>();
            services.RemoveAll<VeadatabaseProductionContext>();

            string connectionString = GetConnectionString();
            services.AddDbContext<DomainModelContext>(options => { options.UseSqlite(connectionString); });
            services.AddDbContext<VeadatabaseProductionContext>(options => { options.UseSqlite(connectionString); });

            SetupCleanDatabase(services);
        });
    }

    private void SetupCleanDatabase(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var domainModelScope = serviceProvider.CreateScope();
        var domainModelContext = domainModelScope.ServiceProvider.GetRequiredService<DomainModelContext>();
        domainModelContext.Database.EnsureDeleted();
        domainModelContext.Database.EnsureCreated();

        using var veaScope = serviceProvider.CreateScope();
        var veaContext = veaScope.ServiceProvider.GetRequiredService<VeadatabaseProductionContext>();
        veaContext.Database.EnsureDeleted();
        veaContext.Database.EnsureCreated();
    }


    private string GetConnectionString()
    {
        string testDbName = "Test" + Guid.NewGuid() + ".db";
        string connectionString = $"Data Source={testDbName};";
        return connectionString;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            using var domainModelScope = _serviceCollection.BuildServiceProvider().CreateScope();
            domainModelScope.ServiceProvider.GetRequiredService<DomainModelContext>().Database.EnsureDeleted();
            domainModelScope.ServiceProvider.GetRequiredService<VeadatabaseProductionContext>().Database.EnsureDeleted();
        }

        base.Dispose(disposing);
    }

}