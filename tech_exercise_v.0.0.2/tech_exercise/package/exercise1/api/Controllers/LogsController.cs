using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetExceptionLogs")]
        public async Task<IActionResult> GetExceptionLogs()
        {
            try
            {
                var result = await _mediator.Send(new GetExceptions()
                {
                });

                Log.ForContext($"{nameof(result)}", result, true).Information("LogsController.GetExceptionLogs returned result");
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }            
        }

        [HttpGet("GetSuccessLogs")]
        public async Task<IActionResult> GetSuccessLogs()
        {
            try
            {
                var result = await _mediator.Send(new GetSuccesses()
                {
                });

                Log.ForContext($"{nameof(result)}", result, true).Information("LogsController.GetSuccessLogs returned result");
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}