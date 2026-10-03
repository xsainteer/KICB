using Application.Abstractions.Persistence;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;

namespace Application.Phones;

/// <summary>
/// Phones are always addressed through their client: a phone of another client is reported as not found.
/// </summary>
public sealed class PhoneService(IClientRepository clients, IPhoneRepository phones, IUnitOfWork unitOfWork)
{
    public async Task<Result<IReadOnlyList<PhoneResponse>>> ListAsync(Guid clientId,
        CancellationToken cancellationToken = default)
    {
        if (!await clients.ExistsAsync(clientId, cancellationToken))
            return ClientErrors.NotFound(clientId);

        var clientPhones = await phones.ListByClientAsync(clientId, cancellationToken);
        return Result<IReadOnlyList<PhoneResponse>>.Success(clientPhones.Select(PhoneResponse.From).ToList());
    }

    public async Task<Result<PhoneResponse>> GetAsync(Guid clientId, Guid phoneId,
        CancellationToken cancellationToken = default)
    {
        var phone = await FindAsync(clientId, phoneId, cancellationToken);
        if (phone is null)
            return PhoneErrors.NotFound(phoneId);

        return PhoneResponse.From(phone);
    }

    public async Task<Result<PhoneResponse>> CreateAsync(Guid clientId, string number, PhoneType type,
        CancellationToken cancellationToken = default)
    {
        if (!await clients.ExistsAsync(clientId, cancellationToken))
            return ClientErrors.NotFound(clientId);

        var phone = new Phone(clientId, number, type);
        phones.Add(phone);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PhoneResponse.From(phone);
    }

    public async Task<Result<PhoneResponse>> UpdateAsync(Guid clientId, Guid phoneId, string number, PhoneType type,
        CancellationToken cancellationToken = default)
    {
        var phone = await FindAsync(clientId, phoneId, cancellationToken);
        if (phone is null)
            return PhoneErrors.NotFound(phoneId);

        phone.Update(number, type);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PhoneResponse.From(phone);
    }

    public async Task<Result> DeleteAsync(Guid clientId, Guid phoneId, CancellationToken cancellationToken = default)
    {
        var phone = await FindAsync(clientId, phoneId, cancellationToken);
        if (phone is null)
            return PhoneErrors.NotFound(phoneId);

        phones.Remove(phone);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Phone?> FindAsync(Guid clientId, Guid phoneId, CancellationToken cancellationToken)
    {
        var phone = await phones.GetByIdAsync(phoneId, cancellationToken);
        return phone?.ClientId == clientId ? phone : null;
    }
}
