namespace Application.Abstractions;

/// <summary>
/// Generates account and card numbers that are not used yet. Numbers are assigned by the bank,
/// never chosen by the client, so they are not part of any request.
/// </summary>
public interface INumberGenerator
{
    Task<string> NewAccountNumberAsync(CancellationToken cancellationToken = default);

    Task<string> NewCardNumberAsync(CancellationToken cancellationToken = default);
}
