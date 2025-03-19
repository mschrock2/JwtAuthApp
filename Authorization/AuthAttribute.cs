using Microsoft.AspNetCore.Authorization;

namespace JwtAuthApp.Authorization
{
    internal class AuthAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Adds scoped string to authorization policies,
        /// runs on application startup
        /// </summary>
        /// <param name="scopes"></param>
        public AuthAttribute(string scopes)
        {
            Policy = scopes; // eg. User:r
        }
    }
}
