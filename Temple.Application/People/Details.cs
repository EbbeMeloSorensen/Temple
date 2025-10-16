using AutoMapper;
using MediatR;
using Temple.Persistence;
using Temple.Persistence.Versioned;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using System.Globalization;

namespace Temple.Application.People
{
    public class Details
    {
        public class Query : IRequest<Result<PersonDto>>
        {
            public Guid Id { get; set; }
            public VersioningParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PersonDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly IUnitOfWorkFactory _unitOfWorkFactory;

            public Handler(
                IMapper mapper,
                IUserAccessor userAccessor,
                IUnitOfWorkFactory unitOfWorkFactory)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
            }

            public async Task<Result<PersonDto>> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
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
                        return Result<PersonDto>.Failure("Invalid time format");
                    }
                }

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    var person = await unitOfWork.People.Get(request.Id);
                    var result = _mapper.Map<PersonDto>(person);

                    return Result<PersonDto>.Success(result);
                }
            }
        }
    }
}