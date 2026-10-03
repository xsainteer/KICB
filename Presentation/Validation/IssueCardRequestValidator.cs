using FluentValidation;
using Presentation.Contracts;

namespace Presentation.Validation;

public sealed class IssueCardRequestValidator : AbstractValidator<IssueCardRequest>
{
    public IssueCardRequestValidator()
    {
        RuleFor(request => request.Type).NotNull().IsInEnum();
    }
}
