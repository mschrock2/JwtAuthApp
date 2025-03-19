using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace JwtAuthApp.Authorization
{
    internal class ScopePolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public ScopePolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        /// <summary>
        /// Policy sets up scope challenge for controller,
        /// runs on controller call
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new ScopeRequirement(policyName));//"User:r"));
            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }
    }
}
