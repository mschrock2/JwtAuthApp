using Microsoft.AspNetCore.Authorization;

namespace JwtAuthApp.Authorization
{
    internal class ScopeRequirement : IAuthorizationRequirement
    {
        public string scope { get; private set; }

        public ScopeRequirement(string scope)
        {
            this.scope = scope;
        }
    }
}
