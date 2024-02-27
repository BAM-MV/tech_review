using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Queries;

namespace StargateUnitTest.Queries;

[TestClass]
public class GetAstronautDutiesByNameTests
{
    [TestMethod]
    public async Task GetAstronautDutiesByName_NoError()
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
            var persons = new List<Person> { new Person { Id = 1, Name = "Jimmy" }, new Person { Id = 2, Name = "Teresa" } };
            var person1Detail = new AstronautDetail { Id = 1, PersonId = 1, CurrentRank = "R1", CurrentDutyTitle = "Commander", CareerStartDate = new DateTime(2023, 4, 5) };
            var astronautDuties = new List<AstronautDuty> 
            { 
                new AstronautDuty{ Id = 1, PersonId = 1, DutyTitle = "Pilot", Rank = "R2", DutyStartDate = new DateTime(2023, 4, 5), DutyEndDate = new  DateTime(2023, 5, 1)}, 
                new AstronautDuty{ Id = 2, PersonId = 1, DutyTitle = "Commander", Rank = "R1", DutyStartDate = new DateTime(2024, 2, 26) }
            };
            await context.People.AddRangeAsync(persons);
            await context.AstronautDetails.AddAsync(person1Detail);
            await context.AstronautDuties.AddRangeAsync(astronautDuties);
            await context.SaveChangesAsync();
        }

        using (var context = new StargateContext(options))
        {
            var handler = new GetAstronautDutiesByNameHandler(context);

            var result = await handler.Handle(new GetAstronautDutiesByName() { Name = "Jimmy"}, default);

            var expectedResult = new GetAstronautDutiesByNameResult
            {
                Person = new PersonAstronaut { PersonId = 1, Name = "Jimmy", CurrentRank = "R1", CurrentDutyTitle = "Commander", CareerStartDate = new DateTime(2023, 4, 5) },
                AstronautDuties = new List<AstronautDutyDto> 
                { 
                    new AstronautDutyDto { DutyTitle = "Pilot", Rank = "R2", DutyStartDate = new DateTime(2023, 4, 5), DutyEndDate = new  DateTime(2023, 5, 1) },
                    new AstronautDutyDto { DutyTitle = "Commander", Rank = "R1", DutyStartDate = new DateTime(2024, 2, 26) },
                },

            };
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
