using Microsoft.EntityFrameworkCore;
using Services;
using VIAPadelClub.Core.Application.Extensions;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using VIAPadelClub.Infrastructure.EfcQueries;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using VIAPadelClub.Presentation.WebApi.Endpoints.Queries;
using VIAPadelClub.Presentation.WebApi.ObjectMapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterCommandHandlers();
builder.Services.RegisterCommandDispatcher();
builder.Services.RegisterQueryDispatcher();
builder.Services.RegisterQueryHandler();
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

builder.Services.AddDbContext<DomainModelContext>(options => options.UseSqlite("Data Source = VEAPadelClub.db"));
builder.Services.AddDbContext<VeadatabaseProductionContext>(options => options.UseSqlite("Data Source = VEAPadelClub.db"));

// Register Mappers
builder.Services.AddScoped<IMapper, ObjectMapper>();
builder.Services.AddScoped<IMappingConfig<ViewManagerOverview.Answer, ViewManagerOverviewResponse>, ViewManagerOverviewAnswerToResponseMapping>();
builder.Services.AddScoped<IMappingConfig<PlayerScheduleOverview.Answer, PlayerScheduleViewResponse>, PlayerScheduleAnswerToPlayerScheduleViewResponseMapping>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
app.UseAuthorization();

app.MapControllers();

app.Run();