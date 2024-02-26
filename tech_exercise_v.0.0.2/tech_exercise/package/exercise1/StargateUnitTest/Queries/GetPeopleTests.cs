using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Queries;

namespace StargateUnitTest.Queries;

[TestClass]
public class GetPeopleHandlerTest
{
    [TestMethod]
    public async Task GetPeople_NoError()
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
            var person1Detail = new AstronautDetail { Id = 1, PersonId = 1, CurrentRank = "R1", CurrentDutyTitle = "Commander", CareerStartDate = new DateTime(2024, 2, 25) };

            context.People.AddRange(persons);
            context.AstronautDetails.Add(person1Detail);
            context.SaveChanges();
        }

        using (var context = new StargateContext(options))
        {
            var handler = new GetPeopleHandler(context);

            var result = await handler.Handle(new GetPeople(), default);

            var expectedResult = new GetPeopleResult
            {
                People = new List<PersonAstronaut>
                {
                    new PersonAstronaut { PersonId = 1, Name = "Jimmy", CurrentRank = "R1", CurrentDutyTitle = "Commander", CareerStartDate = new DateTime(2024, 2, 25) },
                    new PersonAstronaut { PersonId = 2, Name = "Teresa",  }
                }
            };
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
