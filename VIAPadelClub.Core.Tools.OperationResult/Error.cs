namespace VIAPadelClub.Core.Tools.OperationResult;

public class Error(int id, string message)
{
    public int _id { get; set; } = id;
    public string _message { get; set; } = message;
}