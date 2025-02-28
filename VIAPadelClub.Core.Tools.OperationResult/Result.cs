namespace VIAPadelClub.Core.Tools.OperationResult;

public class Result<T>
{
    public bool Success { get; }
    public string ErrorMessage { get; private set; } = null!;
    public T? Data { get; }

    private Result(bool success, string errorMessage, T? data)
    {
        Success = success;
        ErrorMessage = errorMessage;
        Data = data;
    }

    private Result(bool success, T data)
    {
        Success = success;
        Data = data;
    }

    public static Result<T> Ok(T data)
    {
        return new Result<T>(true, data);
    }
    
    public static Result<T> Fail(string message)
    {
        return new Result<T>(false, message, default);
    }
}

public class Result
{
    public bool Success { get; }
    public string ErrorMessage { get; private set; } = null!;

    private Result(bool success, string errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

    private Result(bool success)
    {
        Success = success;
    }

    public static Result Ok()
    {
        return new Result(true);
    }

    public static Result Fail(string message)
    {
        return new Result(false, message);
    }
}