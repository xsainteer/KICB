using FluentValidation;

namespace Presentation.Validation;

public static class MoneyRuleExtensions
{
    /// <summary>
    /// A positive amount with at most two decimal places. Balances are stored as numeric(18,2),
    /// so a finer amount would be silently rounded on save.
    /// </summary>
    public static IRuleBuilderOptions<T, decimal> MoneyAmount<T>(this IRuleBuilder<T, decimal> ruleBuilder) =>
        ruleBuilder
            .GreaterThan(0)
            .PrecisionScale(18, 2, ignoreTrailingZeros: true)
            .WithMessage("'{PropertyName}' must have at most 16 integer digits and 2 decimal places.");
}
