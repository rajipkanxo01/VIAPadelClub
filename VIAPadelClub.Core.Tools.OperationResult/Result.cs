namespace VIAPadelClub.Core.Tools.OperationResult;

public class Result<T>
{
    public bool Success { get; }
    public string ErrorMessage { get; } = string.Empty;
    public T Data { get; }

    // used for success
    private Result(T data)
    {
        Success = true;
        Data = data;
    }

    // used for failure
    private Result(string errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
        Data = default!;
    }

    public static Result<T> Ok(T data)
    {
        return new Result<T>(data);
    }

    public static Result<T> Fail(string message)
    {
        return new Result<T>(message);
    }
}

public class Result
{
    public bool Success { get; }
    public string ErrorMessage { get; } = string.Empty;

    private Result()
    {
        Success = true;
    }

    private Result(string errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
    }

    public static Result Ok()
    {
        return new Result();
    }

    public static Result Fail(string message)
    {
        return new Result(message);
    }
}