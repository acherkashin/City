using System;
using System.Linq;
using System.Security.Claims;

namespace CyberCity.Utils
{
    public static class UserExtentions
    {
        public const string IdClaimType = "ID";

        public static string GetRole(this ClaimsPrincipal principal)
        {
            return principal.Identities.FirstOrDefault()?.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
        }

        public static int GetId(this ClaimsPrincipal principal)
        {
            var id = principal.Identities.FirstOrDefault()?.Claims.FirstOrDefault(claim => claim.Type == UserExtentions.IdClaimType)?.Value;

            return Convert.ToInt32(id);
        }
    }
}
