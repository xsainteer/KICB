using FluentValidation;
using Presentation.Contracts;

namespace Presentation.Validation;

public sealed class OpenAccountRequestValidator : AbstractValidator<OpenAccountRequest>
{
    public OpenAccountRequestValidator()
    {
        RuleFor(request => request.Currency).NotNull().IsInEnum();
    }
}
