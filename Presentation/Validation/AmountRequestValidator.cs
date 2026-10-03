using FluentValidation;
using Presentation.Contracts;

namespace Presentation.Validation;

public sealed class AmountRequestValidator : AbstractValidator<AmountRequest>
{
    public AmountRequestValidator()
    {
        RuleFor(request => request.Amount).MoneyAmount();
    }
}
