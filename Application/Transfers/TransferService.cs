using Application.Abstractions.Persistence;
using Application.Accounts;
using Domain.Common;
using Domain.Errors;

namespace Application.Transfers;

public sealed class TransferService(IAccountRepository accounts, IUnitOfWork unitOfWork)
{
    /// <summary>
    /// Transfers funds between two accounts, of the same client or of different clients, in one transaction.
    /// </summary>
    /// <returns>
    /// The source account after the transfer. The destination may belong to another client,
    /// so its balance is not exposed to the sender.
    /// </returns>
    public async Task<Result<AccountResponse>> TransferAsync(Guid sourceAccountId, Guid destinationAccountId,
        decimal amount, CancellationToken cancellationToken = default)
    {
        var source = await accounts.GetByIdAsync(sourceAccountId, cancellationToken);
        if (source is null)
            return AccountErrors.NotFound(sourceAccountId);

        var destination = await accounts.GetByIdAsync(destinationAccountId, cancellationToken);
        if (destination is null)
            return AccountErrors.NotFound(destinationAccountId);

        var result = source.TransferTo(destination, amount);
        if (result.IsFailure)
            return result.Error!;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AccountResponse.From(source);
    }
}
