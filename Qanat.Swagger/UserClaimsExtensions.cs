using System.Security.Claims;

namespace Qanat.Swagger
{
    public static class UserClaimsExtensions
    {
        public static int? GetUserID(this ClaimsPrincipal user)
        {
            var userIDClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIDClaim, out var userID) ? userID : null;
        }

        public static int? GetRoleID(this ClaimsPrincipal user)
        {
            var roleIdClaim = user.FindFirst("RoleID")?.Value;
            return int.TryParse(roleIdClaim, out var roleId) ? roleId : (int?)null;
        }
    }
}
