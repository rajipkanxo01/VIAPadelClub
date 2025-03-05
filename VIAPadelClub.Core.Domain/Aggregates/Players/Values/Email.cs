using System.Text.RegularExpressions;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.Players.Values
{
    public class Email
    {
        internal string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string email)
        {
            var validationResults = new List<Result>
            {
                ValidateEmailNotEmpty(email),
                ValidateEmailIsCorrectFormat(email),
                ValidateEmailEndsWithViaDk(email)
            };

            // Collect errors from all validations
            var errors = validationResults.Where(r => !r.Success).Select(r => r.ErrorMessage).ToList();

            if (errors.Any())
            {
                return Result<Email>.Fail(string.Join("; ", errors));
            }

            return Result<Email>.Ok(new Email(email));
        }

        private static Result ValidateEmailNotEmpty(string email)
        {
            return string.IsNullOrWhiteSpace(email)
                ? Result.Fail(ErrorMessage.EmailCannotBeEmpty()._message)
                : Result.Ok();
        }

        private static Result ValidateEmailIsCorrectFormat(string email)
        {
            const string emailPattern = @"^([a-zA-Z]{3,4}|\d{6})@via\.dk$";
            return !Regex.IsMatch(email, emailPattern)
                ? Result.Fail(ErrorMessage.InvalidEmailFormat()._message)
                : Result.Ok();
        }

        private static Result ValidateEmailEndsWithViaDk(string email)
        {
            return !email.EndsWith("@via.dk")
                ? Result.Fail(ErrorMessage.EmailMustEndWithViaDk()._message)
                : Result.Ok();
        }
    }
}