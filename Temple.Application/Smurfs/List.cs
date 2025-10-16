using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Temple.Domain.Entities.Smurfs;
using Temple.Persistence;
using Temple.Application.Core;
using Temple.Application.Interfaces;

namespace Temple.Application.Smurfs
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<SmurfDto>>>
        {
            public SmurfParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<SmurfDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IUnitOfWorkFactory _unitOfWorkFactory;
            private readonly IPagingHandler<SmurfDto> _pagingHandler;

            public Handler(
                IMapper mapper,
                IUnitOfWorkFactory unitOfWorkFactory,
                IPagingHandler<SmurfDto> pagingHandler)
            {
                _mapper = mapper;
                _unitOfWorkFactory = unitOfWorkFactory;
                _pagingHandler = pagingHandler;
            }

            public async Task<Result<PagedList<SmurfDto>>> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
                var predicates = new List<Expression<Func<Smurf, bool>>>();

                // if (!string.IsNullOrEmpty(request.Params.Name))
                // {
                //     var filter = request.Params.Name.ToLower();

                //     predicates.Add(x =>
                //         x.Name.ToLower().Contains(filter));
                // }

                // Det her er nok ikke super optimalt sådan rent båndbreddemæssigt....
                // Man finder alle de items, der passer med filteret, dvs det kan i praksis 
                // sagtens være alle items, og så filtrerer man det ned til den side, man er
                // interesseret i bagefter. Det virker for få items men har nok en kedelig
                // performance penalty for mange items.

                // Det kommer sig nok af, at den originale implementation af denne metode
                // gjorde brug af dbcontexten fra EntityFrameworkCore, så der kunne man jo
                // tilføje pagination direkte i forespørgslen til databasen.
                // Da du så sadlede om til et UnitOfWork mønster, så mistede du den mulighed,
                // fordi dit repository interface ikke understøtter pagination,
                // men det kan du jo bringe det til at kunne.

                var smurfs = await unitOfWork.Smurfs.Find(predicates);

                var result = _mapper.Map<IEnumerable<SmurfDto>>(smurfs);

                return Result<PagedList<SmurfDto>>.Success(
                    _pagingHandler.Create(result, request.Params.PageNumber,
                        request.Params.PageSize)
                );
            }
        }
    }
}
