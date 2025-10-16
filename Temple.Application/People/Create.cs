using FluentValidation;
using MediatR;
using Temple.Domain.Entities.PR;
using Temple.Persistence;
using Temple.Persistence.Versioned;
using Temple.Application.Core;
using Temple.Application.Interfaces;

namespace Temple.Application.People
{
    public class Create
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
            private readonly IUserAccessor _userAccessor;
            private readonly IUnitOfWorkFactory _unitOfWorkFactory;

            public Handler(
                IUserAccessor userAccessor,
                IUnitOfWorkFactory unitOfWorkFactory)
            {
                _userAccessor = userAccessor;
                _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
            }

            public async Task<Result<Unit>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    // HUSK AWAIT HER, ELLERS VIRKER DET IKKE!!
                    await unitOfWork.People.Add(request.Person);
                    unitOfWork.Complete();
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}