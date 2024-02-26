using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

namespace StargateUnitTest.Commands;

[TestClass]
public class UpdatePersonTests
{
    [TestMethod]
    public async Task UpdatePersonHandler()
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
            context.People.Add(new Person { Id = 1, Name = "Teresa Gonzales" });
            await context.SaveChangesAsync();
        }

        using (var context = new StargateContext(options))
        {
            var handler = new UpdatePersonHandler(context);

            var result = await handler.Handle(new UpdatePerson() { CurrentName = "Teresa Gonzales", NewName = "Teresa Alvarez" }, default);

            var expectedResult = new UpdatePersonResult
            {
                Id = 1,
            };
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

    [TestMethod]
    public async Task CreatePersonPreProcessorTest_CurrentNameDoesNotExist()
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
            context.People.Add(new Person { Id = 1, Name = "Morgan S" });
            context.People.Add(new Person { Id = 2, Name = "Megan P" });
            await context.SaveChangesAsync();
        }

        using (var context = new StargateContext(options))
        {
            var preProcessor = new UpdatePersonPreProcessor(context);

            var param = new UpdatePerson() { CurrentName = "Teresa Gonzales", NewName = "Megan Gonzales" };
            var act = async () => await preProcessor.Process(param, default);

            await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("Teresa Gonzales does not exist");
        }
    }

    [TestMethod]
    public async Task UpdatePersonPreProcessorTest_NewNameExists()
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
            context.People.Add(new Person { Id = 1, Name = "Teresa Gonzales" });
            context.People.Add(new Person { Id = 2, Name = "Megan Gonzales" });
            await context.SaveChangesAsync();
        }

        using (var context = new StargateContext(options))
        {
            var preProcessor = new UpdatePersonPreProcessor(context);

            var param = new UpdatePerson() { CurrentName = "Teresa Gonzales", NewName = "Megan Gonzales" };
            var act = async () => await preProcessor.Process(param, default);

            await act.Should().ThrowAsync<BadHttpRequestException>()
            .WithMessage("Megan Gonzales already exists");
        }
    }
}
