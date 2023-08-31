using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2;
using System.Security.Claims;

namespace AspNetCoreExternalCallback.Pages
{
    public class CallbackModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // Get the identity exactly as received from the Saml2 Idp.
            var external = await HttpContext.AuthenticateAsync("external");

            if (!external.Succeeded)
            {
                // We should really not be able to get here without an external
                // identity, but in a production scenario some better error handling
                // is adviced.
                throw new InvalidOperationException();
            }

            // Create a new set of claims based on information in the external identity.
            var claims = new List<Claim>
            {
                new Claim("sub", external.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)),
            };

            // This works based on attributes from the StubIdp Tolvan Tolvansson user.
            var givenName = external.Principal!.FindFirstValue("Subject_GivenName");
            var surName = external.Principal!.FindFirstValue("Subject_Surname");

            var name = (givenName + " " + surName).Trim();

            if (!string.IsNullOrEmpty(name))
            {
                claims.Add(new Claim("name", name));
            }

            // The logout nameidentifier and session index claims are required to be able to
            // initiate a Saml2 logout.
            var logoutNameId = external.Principal!.FindFirst(Saml2ClaimTypes.LogoutNameIdentifier);
            var sessionIndex = external.Principal.FindFirst(Saml2ClaimTypes.SessionIndex);

            if (logoutNameId != null && sessionIndex != null)
            {
                claims.Add(logoutNameId);
                claims.Add(sessionIndex);
            }

            foreach (var role in external.Principal!.FindAll(ClaimTypes.Role))
            {
                // Don't just accept roles from the Idp, pick what roles the Idp
                // is allowed to send and translate it to our app role claim type and
                // app role name.
                if (role.Value == "Administrator")
                {
                    claims.Add(new Claim("role", "admin"));
                }
            }

            // Create an identity and principal that is in the right format for our application.
            // Always use the four param version of the ClaimsIdentity constructor
            var identity = new ClaimsIdentity(claims, "Saml2", "name", "role");
            var principal = new ClaimsPrincipal(identity);

            // Sign in to create a session in our application.
            await HttpContext.SignInAsync(principal);
            // Now we are done with the external identity, call signout on it to remove cookie.
            await HttpContext.SignOutAsync("external");

            // Finally redirect to the destination url that was put in the props by login.cshtml.cs
            return Redirect(external.Properties!.Items["returnUrl"]!);
        }
    }
}
