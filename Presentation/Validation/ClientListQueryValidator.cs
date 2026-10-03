using FluentValidation;
using Presentation.Contracts;

namespace Presentation.Validation;

public sealed class ClientListQueryValidator : AbstractValidator<ClientListQuery>
{
    private const int MaxPageSize = 100;

    public ClientListQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, MaxPageSize);

        RuleFor(query => query.BornTo)
            .GreaterThanOrEqualTo(query => query.BornFrom)
            .When(query => query.BornFrom is not null && query.BornTo is not null);
    }
}
