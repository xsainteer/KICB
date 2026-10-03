using FluentValidation;
using Presentation.Contracts;

namespace Presentation.Validation;

public sealed class ClientRequestValidator : AbstractValidator<ClientRequest>
{
    private static readonly DateOnly EarliestDateOfBirth = new(1900, 1, 1);

    public ClientRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(request => request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.MiddleName).MaximumLength(100);

        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(254);

        RuleFor(request => request.DateOfBirth)
            .LessThan(_ => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime))
            .WithMessage("'{PropertyName}' must be in the past.")
            .GreaterThanOrEqualTo(EarliestDateOfBirth);
    }
}
