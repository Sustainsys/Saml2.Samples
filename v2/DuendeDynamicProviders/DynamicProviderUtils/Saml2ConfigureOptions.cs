﻿using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Hosting.DynamicProviders;
using IdentityModel;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.WebSso;
using System.Security.Claims;
using System.Xml.Linq;

namespace DuendeDynamicProviders.DynamicProviderUtils;

// Cannot use the utility base class as that assumes authentication options derives from RemoteAuthenticationOptions
// and Saml2Options doesn't.
class Saml2ConfigureOptions : IConfigureNamedOptions<Saml2Options>
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public Saml2ConfigureOptions(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public void Configure(Saml2Options options) { }

    public void Configure(string name, Saml2Options options)
    {
        // we have to resolve these here due to DI lifetime issues
        var providerOptions = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<DynamicProviderOptions>();
        var cache = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<DynamicAuthenticationSchemeCache>();

        var idp = cache.GetIdentityProvider<Saml2IdentityProvider>(name);
        if (idp != null)
        {
            var pathPrefix = providerOptions.PathPrefix + "/" + idp.Scheme;

            options.SPOptions.EntityId = new EntityId(idp.SPEntityId);
            options.SPOptions.ModulePath = pathPrefix;

            options.SignInScheme = providerOptions.SignInScheme;

            // The Saml2 library presents the subject Id as ClaimTypes.NameIdentifier, IdSrv requires a "sub" claim.
            options.Notifications.AcsCommandResultCreated = RenameNameIdToSub;

            if(options.IdentityProviders.IsEmpty)
            {
                options.IdentityProviders.Add(
                    new Sustainsys.Saml2.IdentityProvider(
                        new EntityId(idp.IdpEntityId), options.SPOptions)
                    {
                        LoadMetadata = true
                    });
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    private void RenameNameIdToSub(CommandResult commandResult, Saml2Response saml2Response)
    { 
        var identity = commandResult.Principal.Identities.Single();

        var nameId = identity.FindFirst(ClaimTypes.NameIdentifier);

        identity.RemoveClaim(nameId);

        identity.AddClaim(new Claim(JwtClaimTypes.Subject, nameId.Value, ClaimValueTypes.String, nameId.Issuer));
    }
}