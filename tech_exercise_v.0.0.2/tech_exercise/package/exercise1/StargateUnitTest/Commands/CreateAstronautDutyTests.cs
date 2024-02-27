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
    [TestMethod]
    public async Task CreateAstronautDutyHandler_NoError()
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
            context.AstronautDuties.Add(new AstronautDuty { Id = 1, PersonId = 1, Rank = "R1", DutyTitle = "Pilot", DutyStartDate = new DateTime(2024, 2, 1) });
            context.AstronautDetails.Add(new AstronautDetail { Id = 1, PersonId = 1, CurrentRank = "R1", CurrentDutyTitle = "Pilot", CareerStartDate = new DateTime(2024, 2, 1) });
            await context.SaveChangesAsync();
        }


        using (var context = new StargateContext(options))
        {
            var handler = new CreateAstronautDutyHandler(context);

            var param = new CreateAstronautDuty() { Name = "Teresa Gonzales", Rank = "R1", DutyTitle = "Commander", DutyStartDate = new DateTime(2024, 2, 26) };
            var result = await handler.Handle(param, default);

            var expectedResult = new CreateAstronautDutyResult
            {
                Id = 2,
            };
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

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
                .WithMessage("Teresa Gonzales already has duty Commander with rank R1 for date 2/26/2024 12:00:00 AM");
        }
    }
}
