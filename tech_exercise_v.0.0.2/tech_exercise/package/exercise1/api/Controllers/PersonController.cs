using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;
using System.Xml.Linq;

namespace StargateAPI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase //component class like hotdogstand or frenchfrie stand
    {
        private readonly IMediator _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople()
                {
                });
                Log.ForContext($"{nameof(result)}", result, true).Information("PersonController.GetPeople returned results");
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

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                var result = await _mediator.Send(new GetPersonByName() //sender
                {
                    Name = name
                });

                Log.ForContext($"{nameof(result)}", result, true).Information($"{ControllerContext.ActionDescriptor.ControllerName}.{ControllerContext.ActionDescriptor.ActionName} returned result");
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                Log.ForContext($"{nameof(name)}", name).Error(ex, ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                var result = await _mediator.Send(new CreatePerson()
                {
                    Name = name
                });
                Log.ForContext($"{nameof(result)}", result, true).Information($"{ControllerContext.ActionDescriptor.ControllerName}.{ControllerContext.ActionDescriptor.ActionName} returned result");
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                Log.ForContext($"{nameof(name)}", name).Error(ex, ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }

        }

        [HttpPut("")]
        public async Task<IActionResult> UpdatePerson([FromBody] UpdatePerson UpdatePersonDto)
        {
            try
            {
                var result = await _mediator.Send(UpdatePersonDto);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
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