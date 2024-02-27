using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetExceptions : IRequest<GetExceptionsResult>
    {
    }

    public class GetExceptionsHandler : IRequestHandler<GetExceptions, GetExceptionsResult>
    {
        private readonly StargateContext _context;

        public GetExceptionsHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetExceptionsResult> Handle(GetExceptions request, CancellationToken cancellationToken)
        {

            var result = new GetExceptionsResult();

            var query = $"SELECT ExceptionLog.Timestamp, ExceptionLog.Level, ExceptionLog.Exception, ExceptionLog.RenderedMessage, ExceptionLog.Properties FROM [ExceptionLog] ExceptionLog order by Timestamp desc";

            var exceptions = await _context.Connection.QueryAsync<LogDto>(query);

            result.Exceptions = exceptions.ToList();

            return result;
        }
    }

    public class GetExceptionsResult : BaseResponse
    {
        public List<LogDto> Exceptions { get; set; } = new List<LogDto>();
    }
}
