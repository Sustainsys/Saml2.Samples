using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AspNetCore.Pages
{
    [Authorize]
    public class SecureModel : PageModel
    {
        public IDictionary<string, string?> Properties { get; set; } = default!;

        public IEnumerable<Claim> Claims { get; set; } = default!;

        public async Task OnGet()
        {
            var authResult = await HttpContext.AuthenticateAsync();

            Properties = authResult.Properties!.Items;
            Claims = authResult.Principal!.Claims;
        }
    }
}
