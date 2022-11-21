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
            // to both terminate the local session and issue a remote signout.
            return SignOut(props, Saml2Defaults.Scheme, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
