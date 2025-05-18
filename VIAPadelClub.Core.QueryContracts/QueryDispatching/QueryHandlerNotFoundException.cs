namespace VIAPadelClub.Core.QueryContracts.QueryDispatching;

public class QueryHandlerNotFoundException : Exception
{
    public QueryHandlerNotFoundException(string queryType, string answerType)
        : base($"Query handler not found for query: {queryType} with expected answer: {answerType}")
    {
    }
}