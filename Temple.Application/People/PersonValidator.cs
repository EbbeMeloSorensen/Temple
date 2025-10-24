using FluentValidation;
using Temple.Domain.Entities.PR;

namespace Temple.Application.People;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.FirstName).MaximumLength(10);
    }
}
