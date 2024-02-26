using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

namespace StargateUnitTest.Commands;

[TestClass]
public class CreateAstronautDutyTests
{
    //[TestMethod]
    //public async Task CreateAstronautDutyTests_NoError()
    //{
    //    var connection = new SqliteConnection("DataSource=:memory:");
    //    connection.Open();

    //    var options = new DbContextOptionsBuilder<StargateContext>().UseSqlite(connection).Options;

    //    using (var context = new StargateContext(options))
    //    {
    //        context.Database.EnsureCreated();
    //    }

    //    using (var context = new StargateContext(options))
    //    {
    //        var handler = new CreatePersonHandler(context);

    //        var result = await handler.Handle(new CreatePerson() { Name = "Teresa Gonzales"}, default);

    //        var expectedResult = new CreatePersonResult
    //        {
    //            Id = 1,
    //        };
    //        result.Should().BeEquivalentTo(expectedResult);
    //    }
    //}

    [TestMethod]
    public async Task CreateAstronautDutyPreProcessor_NoOverlapInDuties()
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
            var preProcessor = new CreateAstronautDutyPreProcessor(context);

            var param = new CreateAstronautDuty() { Name = "Teresa Gonzales", Rank = "R1", DutyTitle = "Commander", DutyStartDate = new DateTime(2024, 2, 26) };
            var act = async () => await preProcessor.Process(param, default);

            await act.Should().NotThrowAsync<BadHttpRequestException>();
        }
    }

    [TestMethod]
    public async Task CreateAstronautDutyPreProcessor_OverlapInDuties()
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
            context.AstronautDuties.Add(new AstronautDuty { Id = 1, PersonId = 1, DutyStartDate = new DateTime(2024, 2, 26), DutyTitle = "Commander", Rank = "R1" });
            await context.SaveChangesAsync();
        }

        using (var context = new StargateContext(options))
        {
            var preProcessor = new CreateAstronautDutyPreProcessor(context);

            var param = new CreateAstronautDuty() { Name = "Teresa Gonzales", Rank = "R1", DutyTitle = "Commander", DutyStartDate = new DateTime(2024, 2, 26) };
            var act = async () => await preProcessor.Process(param, default);

            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Teresa Gonzales already has duty Commander for state date 2/26/2024 12:00:00 AM");
        }
    }
}
