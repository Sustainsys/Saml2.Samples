using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainsys.Saml2.AspNetCore2;

namespace AspNetCore.Pages
{
    public class LoginModel : PageModel
    {
        // Use Post for login button as it is a state change.
        public IActionResult OnPost()
        {
            // Automatic handling is to redirect back to same page after successful
            // login. We don't want that on explicit login.
            var props = new AuthenticationProperties
            {
                RedirectUri = "/"
            };

            return Challenge(props, Saml2Defaults.Scheme);
        }
    }
}
