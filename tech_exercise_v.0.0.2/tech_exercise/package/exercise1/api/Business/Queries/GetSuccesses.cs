using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetSuccesses : IRequest<GetSuccessesResult>
    {
    }

    public class GetSuccessesHandler : IRequestHandler<GetSuccesses, GetSuccessesResult>
    {
        private readonly StargateContext _context;

        public GetSuccessesHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetSuccessesResult> Handle(GetSuccesses request, CancellationToken cancellationToken)
        {

            var result = new GetSuccessesResult();

            var query = $"SELECT SuccessLog.Timestamp, SuccessLog.Level, SuccessLog.Exception, SuccessLog.RenderedMessage, SuccessLog.Properties FROM [SuccessLog] SuccessLog order by Timestamp desc";

            var successes = await _context.Connection.QueryAsync<LogDto>(query);

            result.Successes = successes.ToList();

            return result;
        }
    }

    public class GetSuccessesResult : BaseResponse
    {
        public List<LogDto> Successes { get; set; } = new List<LogDto>();
    }
}
