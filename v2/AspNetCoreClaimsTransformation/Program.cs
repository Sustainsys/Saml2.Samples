using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(opt =>
{
    // Default scheme that maintains session is cookies.
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // If there's a challenge to sign in, use the Saml2 scheme.
    opt.DefaultChallengeScheme = Saml2Defaults.Scheme;
})
.AddCookie()
.AddSaml2(opt =>
{
    // Set up our EntityId, this is our application.
    opt.SPOptions.EntityId = new EntityId("https://localhost:5001/Saml2");

    // Add an identity provider.
    opt.IdentityProviders.Add(new IdentityProvider(
        // The identityprovider's entity id.
        new EntityId("https://stubidp.sustainsys.com/Metadata"),
        opt.SPOptions)
    {
        // Load config parameters from metadata, using the Entity Id as the metadata address.
        LoadMetadata = true,
    });

    // Transform claims using a callback/notification. This is the most simple way to transform
    // claims, but there is no way to show UI and there is no access to other services.
    opt.Notifications.AcsCommandResultCreated = (commandResult, response) =>
    {
        // Grab the signature algorithm from the XML
        var signatureMethod = response.XmlElement
            ["Signature", SignedXml.XmlDsigNamespaceUrl]!
            ["SignedInfo", SignedXml.XmlDsigNamespaceUrl]!
            ["SignatureMethod", SignedXml.XmlDsigNamespaceUrl]!
            .GetAttribute("Algorithm");

        // Get ClaimsIdentity
        var identity = commandResult.Principal.Identities.Single();

        identity.AddClaim(new Claim("SignatureAlgorithm", signatureMethod));

        // Change claim type by removing and adding. This is useful e.g. if we want
        // modern/OIDC-style "sub" claim and not http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
        var nameIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("There should always be a NameId in a Saml2 response...");

        identity.RemoveClaim(nameIdClaim);

        identity.AddClaim(new Claim("sub", nameIdClaim.Value));

        // Also put the somewhat hard to find Idp entity id into a claim by itself.
        identity.AddClaim(new Claim("idp", nameIdClaim.Issuer));
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
