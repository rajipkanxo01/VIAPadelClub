using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace VIAPadelClub.Presentation.WebApi.Endpoints.Common;

[ApiController, Route("api")]
public abstract class EndpointBase : ControllerBase
{

    public static class ApiEndpoint
    {
        public static class WithoutRequest
        {
            public abstract class AndResult<TResult> : EndpointBase
                where TResult : IResult
            {
                public abstract Task<TResult> HandleAsync();
            }

            public abstract class AndResult<TResult, TResult2> : EndpointBase
                where TResult : IResult
                where TResult2 : IResult
            {
                public abstract Task<Results<TResult, TResult2>> HandleAsync();
            }

            public abstract class AndResult<TResult, TResult2, TResult3> : EndpointBase
                where TResult : IResult
                where TResult2 : IResult
                where TResult3 : IResult
            {
                public abstract Task<Results<TResult, TResult2, TResult3>> HandleAsync();
            }
        }

        public abstract class WithRequest<TRequest> : EndpointBase
        {
            public abstract class AndResult<TResult> : EndpointBase
                where TResult : IResult
            {
                public abstract Task<TResult> HandleAsync(TRequest request);
            }

            public abstract class AndResult<TResult, TResult2> : EndpointBase
                where TResult : IResult
                where TResult2 : IResult
            {
                public abstract Task<Results<TResult, TResult2>> HandleAsync(TRequest request);
            }

            public abstract class AndResult<TResult, TResult2, TResult3> : EndpointBase
                where TResult : IResult
                where TResult2 : IResult
                where TResult3 : IResult
            {
                public abstract Task<Results<TResult, TResult2, TResult3>> HandleAsync(TRequest request);
            }
        }
    }
}