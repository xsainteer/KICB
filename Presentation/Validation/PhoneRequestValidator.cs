using FluentValidation;
using Presentation.Contracts;

namespace Presentation.Validation;

public sealed class PhoneRequestValidator : AbstractValidator<PhoneRequest>
{
    public PhoneRequestValidator()
    {
        // E.164-like: optional '+', then 7 to 15 digits, no spaces or separators.
        RuleFor(request => request.Number)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{7,15}$")
            .WithMessage("'{PropertyName}' must be 7 to 15 digits, optionally starting with '+'.");

        RuleFor(request => request.Type).NotNull().IsInEnum();
    }
}
