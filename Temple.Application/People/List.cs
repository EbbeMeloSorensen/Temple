using AutoMapper;
using MediatR;
using Temple.Domain.Entities.PR;
using Temple.Persistence;
using Temple.Persistence.Versioned;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using System.Globalization;
using System.Linq.Expressions;

namespace Temple.Application.People
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<PersonDto>>>
        {
            public PersonParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<PersonDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IUnitOfWorkFactory _unitOfWorkFactory;
            private readonly IPagingHandler<PersonDto> _pagingHandler;

            public Handler(
                IMapper mapper,
                IUnitOfWorkFactory unitOfWorkFactory,
                IPagingHandler<PersonDto> pagingHandler)
            {
                _mapper = mapper;
                _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
                _pagingHandler = pagingHandler;
            }

            public async Task<Result<PagedList<PersonDto>>> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                if (!string.IsNullOrEmpty(request.Params.HistoricalTime))
                {
                    try
                    {
                        var dbTime = DateTime.ParseExact(request.Params.HistoricalTime, "yyyy-MM-ddTHH:mm:ssZ",
                            CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                        (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.HistoricalTime = dbTime;
                    }
                    catch (Exception e)
                    {
                        return Result<PagedList<PersonDto>>.Failure("Invalid time format");
                    }
                }

                if (request.Params.IncludeHistoricalObjects.HasValue)
                {
                    (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.IncludeHistoricalObjects = request.Params.IncludeHistoricalObjects.Value;
                }

                if (!string.IsNullOrEmpty(request.Params.DatabaseTime))
                {
                    try
                    {
                        var dbTime = DateTime.ParseExact(request.Params.DatabaseTime, "yyyy-MM-ddTHH:mm:ssZ",
                            CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                        (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.DatabaseTime = dbTime;
                    }
                    catch (Exception e)
                    {
                        return Result<PagedList<PersonDto>>.Failure("Invalid time format");
                    }
                }

                using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

                var predicates = new List<Expression<Func<Person, bool>>>();

                if (!string.IsNullOrEmpty(request.Params.Name))
                {
                    var filter = request.Params.Name.ToLower();

                    predicates.Add(x =>
                        x.FirstName.ToLower().Contains(filter) ||
                        (!string.IsNullOrEmpty(x.Surname) && x.Surname.ToLower().Contains(filter)));
                }

                var people = await unitOfWork.People.Find(predicates);

                var result = _mapper.Map<IEnumerable<PersonDto>>(people);

                return Result<PagedList<PersonDto>>.Success(
                    _pagingHandler.Create(result, request.Params.PageNumber,
                        request.Params.PageSize)
                );
            }
        }
    }
}