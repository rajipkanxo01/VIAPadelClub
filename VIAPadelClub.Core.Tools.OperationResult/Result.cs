namespace VIAPadelClub.Core.Tools.OperationResult;

public class Result
{
    public bool _success { get; }
    public Error? _error { get; }

    protected Result(bool success, Error? error = null)
    {
        _success = success;
        _error = error;
    }

    public static Result Success() => new(true);
    public static Result Fail(Error error) => new(false, error);
}

public class Result<T> : Result
{
    public T? _data { get; }

    private Result(bool success, T? data, Error? error)
        : base(success, error)
    {
        _data = data;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Fail(Error error) => new(false, default, error);
}

