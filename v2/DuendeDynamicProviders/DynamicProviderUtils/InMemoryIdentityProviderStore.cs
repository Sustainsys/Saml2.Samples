using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;

namespace DuendeDynamicProviders.DynamicProviderUtils;

// The InMemoryOidcProviderStore supplied by the Duende package only
// supports Oidc providers, let's add a generic one.
public class InMemoryIdentityProviderStore : IIdentityProviderStore
{
    private readonly IEnumerable<IdentityProvider> providers;

    public InMemoryIdentityProviderStore(IEnumerable<IdentityProvider> providers)
    {
        this.providers = providers;
    }

    public Task<IEnumerable<IdentityProviderName>> GetAllSchemeNamesAsync() =>
        Task.FromResult(providers.Select(p => new IdentityProviderName
        {
            DisplayName = p.DisplayName,
            Enabled = p.Enabled,
            Scheme = p.Scheme,
        }));
    
    public Task<IdentityProvider> GetBySchemeAsync(string scheme) => 
        Task.FromResult(providers.Single(p => p.Scheme == scheme));
}
