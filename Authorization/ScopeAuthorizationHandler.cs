using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JwtAuthApp.Authorization
{
    internal class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
    {
        private readonly ILogger<ScopeAuthorizationHandler> _logger;

        public ScopeAuthorizationHandler(ILogger<ScopeAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Searches user token claim Scopes for required scope,
        /// runs on controller call
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
        {
            var mustPair = requirement.scope.Split(':'); // eg. User:r
            var mustKey = mustPair[0];
            var mustValue = mustPair[1];
            var claim = context.User.FindFirst(c => c.Type.Equals("Scopes"))?.Value + "";
            var scopes = claim.Replace(';', ',').Split(',');
            foreach (var scope in scopes)
            {
                var pair = scope.Replace('=', ':').Split(':');
                if (pair[0].Equals(mustKey, StringComparison.OrdinalIgnoreCase) && // token has scope (eg. User)
                    pair[1].Contains(mustValue, StringComparison.OrdinalIgnoreCase)) // token scope has value (eg. r)
                {
                    context.Succeed(requirement);
                    break;
                }
            }
            return Task.CompletedTask;
        }
    }
}
