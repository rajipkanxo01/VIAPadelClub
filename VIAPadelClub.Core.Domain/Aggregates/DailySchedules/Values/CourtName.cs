﻿using System.Text.RegularExpressions;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;

public class CourtName
{
    internal string Value { get; }

    private CourtName(string value)
    {
        Value = value;
    }
    
    public static Result<CourtName> Create(string name)
    {
        var validationResults = new List<Result>
        {
            ValidateStartingLetter(name),
            ValidateEndingNumber(name),
            ValidateLength(name)
        };

        // Collect errors from all validations
        var errors = validationResults.Where(r => !r.Success).Select(r => r.ErrorMessage).ToList();

        if (errors.Any())
        {
            return Result<CourtName>.Fail(string.Join("; ", errors));
        }

        return Result<CourtName>.Ok(new CourtName(name));
    }
    
    private static Result ValidateStartingLetter(string name)
    {
        if (name.Length > 0 && (name.StartsWith('s') || name.StartsWith('S') || name.StartsWith('d') || name.StartsWith('D')))
        {
            return Result.Ok();
        }

        return Result.Fail(ErrorMessage.InvalidStartingLetter()._message);
    }

    private static Result ValidateEndingNumber(string name)
    {
        const string pattern = @"^[a-zA-Z]*(10|[1-9])$";
    
        if (Regex.IsMatch(name, pattern))
        {
            return Result.Ok();
        }
    
        return Result.Fail(ErrorMessage.InvalidEndingNumber()._message);
    }

    
    private static Result ValidateLength(string name)
    {
        return (name.Length == 2 || name.Length == 3)
            ? Result.Ok()
            : Result.Fail(ErrorMessage.InvalidLength()._message);
    }

}

