using Application.Accounts;
using Application.Cards;
using Application.Clients;
using Application.Phones;
using Application.Transfers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<ClientService>();
        services.AddScoped<PhoneService>();
        services.AddScoped<AccountService>();
        services.AddScoped<CardService>();
        services.AddScoped<TransferService>();

        return services;
    }
}
