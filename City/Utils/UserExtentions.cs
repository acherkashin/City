using System.Linq;
using System.Security.Claims;

namespace CyberCity.Utils
{
    public static class UserExtentions
    {
        public static string GetRole(this ClaimsPrincipal principal)
        {
            return principal.Identities.FirstOrDefault()?.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
        }
    }
}
