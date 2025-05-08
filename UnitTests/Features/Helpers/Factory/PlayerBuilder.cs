using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers.Factory;

public class PlayerBuilder
{
    private string _email = "111111@via.dk";
    private string _firstName = "Test";
    private string _lastName = "Student";
    private string _profileUri = "https://via.dk/default.jpg";

    private bool _makeVip = false;
    private bool _blacklist = false;
    private bool _quarantine = false;
    
    
    private IEmailUniqueChecker _emailChecker = new FakeUniqueEmailChecker();
    private IScheduleFinder _scheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

    private DateOnly _quarantineStartDate = DateOnly.FromDateTime(DateTime.Today);
    private List<DailySchedule> _quarantineSchedules = new();
    private static FakeDailyScheduleRepository dailyScheduleRepository;

    private PlayerBuilder() { }

    public static PlayerBuilder CreateValid() => new();

    public PlayerBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public PlayerBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }

    public PlayerBuilder WithProfileUri(string uri)
    {
        _profileUri = uri;
        return this;
    }

    public PlayerBuilder WithVIP()
    {
        _makeVip = true;
        return this;
    }

    public PlayerBuilder WithBlacklisted()
    {
        _blacklist = true;
        return this;
    }

    public PlayerBuilder WithQuarantine(DateOnly? startDate = null, List<DailySchedule>? schedules = null)
    {
        _quarantine = true;
        if (startDate != null) _quarantineStartDate = startDate.Value;
        if (schedules != null) _quarantineSchedules = schedules;
        return this;
    }

    public PlayerBuilder WithEmailChecker(IEmailUniqueChecker checker)
    {
        _emailChecker = checker;
        return this;
    }

    public PlayerBuilder WithScheduleFinder(IScheduleFinder finder)
    {
        _scheduleFinder = finder;
        return this;
    }

    public async Task<Result<Player>> BuildAsync()
    {
        var emailResult = Email.Create(_email);
        var nameResult = FullName.Create(_firstName, _lastName);
        var uriResult = ProfileUri.Create(_profileUri);
        
        if (!emailResult.Success)
        {
            return Result<Player>.Fail(emailResult.ErrorMessage);
        }
        
        if (!nameResult.Success)
        {
            return Result<Player>.Fail(nameResult.ErrorMessage);
        }
        
        if (!uriResult.Success)
        {
            return Result<Player>.Fail(uriResult.ErrorMessage);
        }
        
        var playerResult = await Player.Register(emailResult.Data, nameResult.Data, uriResult.Data, _emailChecker);
        // _emailChecker.AddEmail(_email.ToLower());
        
        if (!playerResult.Success)
        {
            return Result<Player>.Fail(playerResult.ErrorMessage);
        }

        var player = playerResult.Data;

        if (_quarantine)
        {
            player.Quarantine(_quarantineStartDate, _quarantineSchedules);
        }

        if (_makeVip)
        {
            player.ChangeToVipStatus();
        }

        if (_blacklist)
        {
            player.Blacklist(_scheduleFinder);
        }

        return Result<Player>.Ok(player);
    }
}
