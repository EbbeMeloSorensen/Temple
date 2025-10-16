using AutoMapper;
using FluentValidation;
using MediatR;
using Temple.Domain.Entities.PR;
using Temple.Persistence;
using Temple.Persistence.Versioned;
using Temple.Application.Core;

namespace Temple.Application.People
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Person Person { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Person).SetValidator(new PersonValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IMapper _mapper;
            private readonly IUnitOfWorkFactory _unitOfWorkFactory;

            public Handler(
                IMapper mapper,
                IUnitOfWorkFactory unitOfWorkFactory)
            {
                _mapper = mapper;
                _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
            }

            public async Task<Result<Unit>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                try
                {
                    using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
                    await unitOfWork.People.Update(request.Person);
                    unitOfWork.Complete();
                }
                catch (Exception e)
                {
                    return Result<Unit>.Failure($"Error editing person: {e.Message}");
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}