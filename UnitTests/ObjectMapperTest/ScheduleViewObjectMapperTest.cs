using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using VIAPadelClub.Presentation.WebApi.ObjectMapping;
using Xunit;

namespace UnitTests.ObjectMapperTest;

public class ScheduleViewObjectMapperTest
{
    private readonly IMapper _mapper;

    public ScheduleViewObjectMapperTest()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapper, ObjectMapper>();
        services.AddSingleton<IMappingConfig<DailySchedule, ViewManagerOverview.ScheduleView>, ScheduleToScheduleViewMapping>();

        var provider = services.BuildServiceProvider();
        _mapper = new ObjectMapper(provider);
    }

    [Fact]
    public void Map_AnonymousToScheduleView_ShouldMapByConvention()
    {
        // Arrange
        var source = new
        {
            Id = "b6d7f8e9-a0c1-b2d3-e4f5-a6b7c8d9eaf0",
            Date = "2025-04-09",
            Status = "active",
            CourtCount = 4
        };

        // Act
        var result = _mapper.Map<ViewManagerOverview.ScheduleView>(source);

        // Assert
        Assert.Equal("b6d7f8e9-a0c1-b2d3-e4f5-a6b7c8d9eaf0", result.Id);
        Assert.Equal("2025-04-09", result.Date);
        Assert.Equal("active", result.Status);
        Assert.Equal(4, result.CourtCount);
    }

    [Fact]
    public void SpecificMapping_DailyScheduleToScheduleView_ShouldUseScheduleToScheduleViewMapping()
    {
        // Arrange
        var dailySchedule = new DailySchedule
        {
            ScheduleId = "sch-002",
            ScheduleDate = "2025-06-01",
            Status = "active",
            Courts = new List<Court>
            {
                new Court { CourtName = "S1" },
                new Court { CourtName = "S2" },
                new Court { CourtName = "S3" }
            }
        };

        // Act
        var result = _mapper.Map<ViewManagerOverview.ScheduleView>(dailySchedule);

        // Assert
        Assert.Equal("sch-002", result.Id);
        Assert.Equal("2025-06-01", result.Date);
        Assert.Equal("active", result.Status);
        Assert.Equal(3, result.CourtCount); 
    }
}
