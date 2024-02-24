using MediatR;
using Moq;
using Moq.AutoMock;
using StargateAPI.Business.Queries;
using StargateAPI.Controllers;

namespace StargateUnitTest;

[TestClass]
public class UnitTest1
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
    public async Task TestMethod1()
    {
        _mediator.Setup(x => x.Send(It.IsAny<GetPeople>(), default))
            .ReturnsAsync(new GetPeopleResult());

        await _personController.GetPeople();

        _mediator.Verify(x => x.Send(It.IsAny<GetPeople>(), default), Times.Once);
    }
}