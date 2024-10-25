using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyMovieWeb.Presentation.Auth0
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(
          AuthorizationHandlerContext context,
          HasScopeRequirement requirement
        )
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
                return Task.CompletedTask;

            // Get permissions in claims
            var permissions = context.User.FindAll(c => c.Type == "permissions").Select(c => c.Value);

            // Succeed if the permissions array contains the required scope
            if (permissions.Any(p => p == requirement.Scope))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
