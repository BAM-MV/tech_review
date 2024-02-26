using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Queries;

namespace StargateUnitTest.Commands;

[TestClass]
public class CreatePersonTests
{
    [TestMethod]
    public async Task CreatePersonHandlerTest_NewPerson()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<StargateContext>().UseSqlite(connection).Options;

        using (var context = new StargateContext(options))
        {
            context.Database.EnsureCreated();
        }

        using (var context = new StargateContext(options))
        {
            var handler = new CreatePersonHandler(context);

            var result = await handler.Handle(new CreatePerson() { Name = "Teresa"}, default);

            var expectedResult = new CreatePersonResult
            {
                Id = 1,
            };
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

    [TestMethod]
    public async Task CreatePersonPreProcessorTest_PersonDoesNotExist()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<StargateContext>().UseSqlite(connection).Options;

        using (var context = new StargateContext(options))
        {
            context.Database.EnsureCreated();
        }

        using (var context = new StargateContext(options))
        {
            var preProcessor = new CreatePersonPreProcessor(context);

            var param = new CreatePerson() { Name = "Teresa" };
            var task = preProcessor.Process(param, default);
            await task;
            task.IsCompleted.Should().BeTrue();
        }
    }

    [TestMethod]
    public async Task CreatePersonPreProcessorTest_PersonExists()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<StargateContext>().UseSqlite(connection).Options;

        using (var context = new StargateContext(options))
        {
            context.Database.EnsureCreated();
        }

        using (var context = new StargateContext(options))
        {
            context.People.Add(new Person { Id = 1, Name = "Teresa" });
            await context.SaveChangesAsync();
        }

        using (var context = new StargateContext(options))
        {
            var preProcessor = new CreatePersonPreProcessor(context);

            var param = new CreatePerson() { Name = "Teresa" };
            var act = async () => await preProcessor.Process(param, default);

            await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("This person already exists");
        }
    }
}
