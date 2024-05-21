using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using DuendeDynamicProviders.DynamicProviderUtils;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;

// By convention all these methods are in the MS namespace to be available
// to intellisense.
namespace Microsoft.Extensions.DependencyInjection;

public static class IdentityServerBuilderExtensions
{
    public static IIdentityServerBuilder AddInMemoryIdentityProviders(
        this IIdentityServerBuilder builder, IEnumerable<IdentityProvider> identityProviders)
    {
        builder.Services.AddSingleton(identityProviders);
        builder.AddIdentityProviderStore<InMemoryIdentityProviderStore>();

        return builder;
    }

    public static IIdentityServerBuilder AddSaml2DynamicProvider(this IIdentityServerBuilder builder)
    {
        builder.Services.Configure<IdentityServerOptions>(opt =>
        {
            opt.DynamicProviders.AddProviderType<Saml2Handler, Saml2Options, Saml2IdentityProvider>("Saml2");
        });

        builder.Services.AddSingleton<IConfigureOptions<Saml2Options>, Saml2ConfigureOptions>();

        // These services are normally registered when AddAuthentication().AddSaml2() is called. But when using dynamic providers
        // we don't call AddSaml() so we have to ensure the services are registered.
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<Saml2Options>, PostConfigureSaml2Options>());
        builder.Services.TryAddTransient<Saml2Handler>();

        return builder;
    }
}
