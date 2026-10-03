using FluentValidation;
using Presentation.Contracts;

namespace Presentation.Validation;

public sealed class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(request => request.SourceAccountId).NotEmpty();

        RuleFor(request => request.DestinationAccountId)
            .NotEmpty()
            .NotEqual(request => request.SourceAccountId)
            .WithMessage("Cannot transfer from an account to itself.");

        RuleFor(request => request.Amount).MoneyAmount();
    }
}
