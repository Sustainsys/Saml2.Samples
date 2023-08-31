using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainsys.Saml2.AspNetCore2;

namespace AspNetCoreExternalCallback.Pages;

public class LoginModel : PageModel
{
    public IActionResult OnGet(string returnUrl)
    {
        returnUrl ??= "/";
        if(!Url.IsLocalUrl(returnUrl))
        {
            throw new InvalidOperationException("Open redirect protection");
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = "/Callback",
            Items = { { "returnUrl", returnUrl } }
        };

        return Challenge(props, Saml2Defaults.Scheme);
    }
}
