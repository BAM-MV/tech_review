using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person is null) throw new BadHttpRequestException($"{request.Name} does not exist");

            var verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate);

            if (verifyNoPreviousDuty is not null) throw new BadHttpRequestException($"{request.Name} already has duty {request.DutyTitle} for state date {request.DutyStartDate}");

            return Task.CompletedTask;
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                var query = $"SELECT * FROM [Person] WHERE \'{request.Name}\' = Name";

                var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query);

                if (person == default)
                {
                    return new CreateAstronautDutyResult()
                    {
                        Success = false,
                        Message = $"{request.Name} does not exist",
                    };
                }

                query = $"SELECT * FROM [AstronautDetail] WHERE {person.Id} = PersonId";

                var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(query);

                if (astronautDetail == null)
                {
                    astronautDetail = new AstronautDetail();
                    astronautDetail.PersonId = person.Id;
                    astronautDetail.CurrentDutyTitle = request.DutyTitle;
                    astronautDetail.CurrentRank = request.Rank;
                    astronautDetail.CareerStartDate = request.DutyStartDate.Date;
                    if (request.DutyTitle == "RETIRED")
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                    }

                    await _context.AstronautDetails.AddAsync(astronautDetail);

                }
                else
                {
                    astronautDetail.CurrentDutyTitle = request.DutyTitle;
                    astronautDetail.CurrentRank = request.Rank;
                    if (request.DutyTitle == "RETIRED")
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                    }
                    _context.AstronautDetails.Update(astronautDetail);
                }

                await _context.SaveChangesAsync();

                query = $"SELECT * FROM [AstronautDuty] WHERE {person.Id} = PersonId Order By DutyStartDate Desc";

                var astronautDuty = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDuty>(query);

                if (astronautDuty != null)
                {
                    astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                    _context.AstronautDuties.Update(astronautDuty);
                }

                var newAstronautDuty = new AstronautDuty()
                {
                    PersonId = person.Id,
                    Rank = request.Rank,
                    DutyTitle = request.DutyTitle,
                    DutyStartDate = request.DutyStartDate.Date,
                    DutyEndDate = null
                };

                await _context.AstronautDuties.AddAsync(newAstronautDuty);

                await _context.SaveChangesAsync();

                dbContextTransaction.Commit();

                return new CreateAstronautDutyResult()
                {
                    Id = newAstronautDuty.Id
                };
            }
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
