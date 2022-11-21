using Duende.IdentityServer.Models;

namespace DuendeDynamicProviders.DynamicProviderUtils;

public class Saml2IdentityProvider : IdentityProvider
{
    public Saml2IdentityProvider() : base("Saml2")
    { }

    public string SPEntityId
    {
        get => this["SPEntityId"];
        set => this["SPEntityId"] = value;
    }

    public string IdpEntityId
    {
        get => this["IdpEntityId"];
        set => this["IdpEntityId"] = value;
    }
}
