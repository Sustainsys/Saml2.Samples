using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainsys.Saml2.AspNetCore2;

namespace AspNetCore.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "/"
            };

            // On application initiated signout, it's the application's responsibility
            // to both terminate the local session and issue a remote signout. Always
            // put the cookie scheme first as that requires headers to be written. If the
            // Saml2 logout uses POST binding it will write the body and flush the headers,
            // causing an exception when the cookie handler tries to write headers.
            return SignOut(props, CookieAuthenticationDefaults.AuthenticationScheme, Saml2Defaults.Scheme);
        }
    }
}
