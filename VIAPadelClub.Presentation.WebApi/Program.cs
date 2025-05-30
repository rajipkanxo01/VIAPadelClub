using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Application.Extensions;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using VIAPadelClub.Infrastructure.EfcQueries;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterHandlers();
builder.Services.RegisterDispatcher();
builder.Services.RegisterQueryHandler();

builder.Services.AddDbContext<DomainModelContext>(options => options.UseSqlite("Data Source = VEAPadelClub.db"));
builder.Services.AddDbContext<VeadatabaseProductionContext>(options => options.UseSqlite("Data Source = VEAPadelClub.db"));

builder.Services.AddScoped<IMapper, ObjectMapper>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();