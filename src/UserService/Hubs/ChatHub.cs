using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace UserService.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserIdFromClaims();
            if (userId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.Value.ToString());
            }
            
            await base.OnConnectedAsync();
        }

        protected long? GetUserIdFromClaims()
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
                var claim = Context.User.FindFirst(t)?.Value;
                if (!string.IsNullOrEmpty(claim) && long.TryParse(claim, out var id)) return id;
            }
            foreach (var c in Context.User.Claims)
            {
                if (!string.IsNullOrEmpty(c.Value) && long.TryParse(c.Value, out var id)) return id;
            }
            return null;
        }
    }
}
