using Application.Abstractions.Persistence;
using Application.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ClientRepository(AppDbContext context) : IClientRepository
{
    public Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Clients.SingleOrDefaultAsync(client => client.Id == id, cancellationToken);

    public async Task<PagedResult<Client>> ListAsync(ClientFilter filter, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var query = Filter(context.Clients.AsNoTracking(), filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(client => client.LastName)
            .ThenBy(client => client.FirstName)
            .ThenBy(client => client.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Client>(items, page, pageSize, totalCount);
    }

    public Task<bool> IsEmailTakenAsync(string email, Guid? exceptClientId = null,
        CancellationToken cancellationToken = default) =>
        context.Clients.AnyAsync(
            client => client.Email == email && client.Id != exceptClientId,
            cancellationToken);

    public void Add(Client client) => context.Clients.Add(client);

    public void Remove(Client client) => context.Clients.Remove(client);

    private static IQueryable<Client> Filter(IQueryable<Client> query, ClientFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.LastName))
            query = query.Where(client => EF.Functions.ILike(client.LastName, ContainsPattern(filter.LastName)));
        if (!string.IsNullOrWhiteSpace(filter.FirstName))
            query = query.Where(client => EF.Functions.ILike(client.FirstName, ContainsPattern(filter.FirstName)));
        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(client => EF.Functions.ILike(client.Email, ContainsPattern(filter.Email)));
        if (filter.BornFrom is { } bornFrom)
            query = query.Where(client => client.DateOfBirth >= bornFrom);
        if (filter.BornTo is { } bornTo)
            query = query.Where(client => client.DateOfBirth <= bornTo);

        return query;
    }

    /// <summary>
    /// Builds an ILIKE substring pattern, escaping the wildcards so user input matches literally
    /// (backslash is PostgreSQL's default LIKE escape character).
    /// </summary>
    private static string ContainsPattern(string value)
    {
        var escaped = value.Trim()
            .Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_");

        return $"%{escaped}%";
    }
}
