using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Use the cookie scheme as default scheme, even for challenge. We need the
// challenge to go through a page under our control to wire up the external login
// callback pattern.
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", opt =>
    {
        // When challenged, redirect to this path. This captures
        // any page tried as a returnUrl query string parameter.
        opt.LoginPath = "/Login";
    })
    .AddCookie("external")
    .AddSaml2(Saml2Defaults.Scheme, opt =>
    {
        // When Saml2 is done, it should persist the resulting identity
        // in the external cookie.
        opt.SignInScheme = "external";

        // Set up our EntityId, this is our application.
        opt.SPOptions.EntityId = new EntityId("https://localhost:5001/Saml2");

        // Single logout messages should be signed according to the SAML2 standard, so we need
        // to add a certificate for our app to sign logout messages with to enable logout functionality.
        opt.SPOptions.ServiceCertificates.Add(new X509Certificate2("Sustainsys.Saml2.Tests.pfx"));

        // Add an identity provider.
        opt.IdentityProviders.Add(new IdentityProvider(
            // The identityprovider's entity id.
            new EntityId("https://stubidp.sustainsys.com/Metadata"),
            opt.SPOptions)
        {
            // Load config parameters from metadata, using the Entity Id as the metadata address.
            LoadMetadata = true
        });
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
