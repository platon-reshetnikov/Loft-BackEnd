using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductService.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected long? GetUserId()
        {
            var tryTypes = new[]
            {
            ClaimTypes.NameIdentifier,
            "nameid",
            JwtRegisteredClaimNames.Sub,
            "id",
            "user_id",
            ClaimTypes.Name,
            ClaimTypes.Email
        };

            foreach (var t in tryTypes)
            {
                var claim = User.FindFirst(t)?.Value;
                if (!string.IsNullOrEmpty(claim) && long.TryParse(claim, out var id)) return id;
            }

            foreach (var c in User.Claims)
            {
                if (!string.IsNullOrEmpty(c.Value) && long.TryParse(c.Value, out var id)) return id;
            }

            return null;
        }

        protected bool IsModerator()
        {
            var roles = GetUserRoles();
            return roles.Contains("MODERATOR"); 
        }

        protected List<string> GetUserRoles()
        {
            return User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        }
    }
}
