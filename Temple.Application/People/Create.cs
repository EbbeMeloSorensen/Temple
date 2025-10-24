using FluentValidation;
using MediatR;
using Craft.Domain;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.PR;
using Temple.Persistence;
using Temple.Persistence.Versioned;

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
            private readonly IBusinessRuleCatalog _businessRuleCatalog;
            private readonly IUnitOfWorkFactory _unitOfWorkFactory;

            public Handler(
                IUserAccessor userAccessor,
                IBusinessRuleCatalog businessRuleCatalog,
                IUnitOfWorkFactory unitOfWorkFactory)
            {
                _userAccessor = userAccessor;
                _businessRuleCatalog = businessRuleCatalog;
                _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
            }

            public async Task<Result<Unit>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                // Hack for now
                request.Person.Start = DateTime.UtcNow;
                request.Person.End = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

                var errors = _businessRuleCatalog.ValidateAtomic(request.Person);

                if (errors.Any())
                {
                    return Result<Unit>.Failure($"Business rule violations detected. First: {errors.First().Value}");
                }

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    await unitOfWork.People.Add(request.Person);
                    unitOfWork.Complete();
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}