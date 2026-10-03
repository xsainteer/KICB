using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class PhoneRepository(AppDbContext context) : IPhoneRepository
{
    public Task<Phone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Phones.SingleOrDefaultAsync(phone => phone.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Phone>> ListByClientAsync(Guid clientId,
        CancellationToken cancellationToken = default) =>
        await context.Phones
            .AsNoTracking()
            .Where(phone => phone.ClientId == clientId)
            .OrderBy(phone => phone.Type)
            .ThenBy(phone => phone.Number)
            .ToListAsync(cancellationToken);

    public void Add(Phone phone) => context.Phones.Add(phone);

    public void Remove(Phone phone) => context.Phones.Remove(phone);
}
