using MediatR;
using Moq;
using Moq.AutoMock;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Queries;
using StargateAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace StargateUnitTest;

[TestClass]
public class PersonControllerTests
{
    private AutoMocker _mocker;
    private Mock<IMediator> _mediator;
    private PersonController _personController;

    [TestInitialize]
    public void SetUp()
    {
        _mocker = new AutoMocker();
        _mediator = _mocker.GetMock<IMediator>();
        _personController = _mocker.CreateInstance<PersonController>();
    }

    [TestMethod]
    public async Task GetPeople_NoError()
    {
        var expectedResult = new GetPeopleResult()
        {
            People = new List<PersonAstronaut>()
            {
                new PersonAstronaut() { PersonId = 1, Name = "Jimmy" },
                new PersonAstronaut() { PersonId = 2, Name = "John" },
            }
        };

        _mediator.Setup(x => x.Send(It.IsAny<GetPeople>(), default))
            .ReturnsAsync(expectedResult);

        var response = await _personController.GetPeople() as ObjectResult;

        _mediator.Verify(x => x.Send(It.IsAny<GetPeople>(), default), Times.Once);
        response?.Value.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public async Task GetPeople_Error()
    {
        var expectedResult = new BaseResponse
        {
            Message = "Bad Request",
            Success = false,
            ResponseCode = (int)HttpStatusCode.InternalServerError,
        };

        _mediator.Setup(x => x.Send(It.IsAny<GetPeople>(), default))
            .ThrowsAsync(new BadHttpRequestException("Bad Request"));

        var response = await _personController.GetPeople() as ObjectResult;

        _mediator.Verify(x => x.Send(It.IsAny<GetPeople>(), default), Times.Once);
        response?.Value.Should().BeEquivalentTo(expectedResult);
    }
}