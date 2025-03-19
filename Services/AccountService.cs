using JwtAuthApp.Helpers;
using JwtAuthApp.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Security.Policy;

namespace JwtAuthApp.Services
{
    public interface IAccountService
    {
        ApiResponse<AccountCreateResponse> Create(string username, string password, string userType = "user", string displayName = "DisplayName", UserStatus userStatus = UserStatus.Active);
        ApiResponse<AccountLogonResponse> Logon(AccountRequest accountRequest);
        ApiResponse<SetStatusChange> SetStatus(SetStatusChange setStatusRequest);
        ApiResponse<SpecificUser> Delete(SpecificUser specificUser);
    }
    public class AccountService : IAccountService
    {
        private const string databaseName = "bty0Db";
        private const string containerName = "user";

        private IAuthHelper authHelpers;
        private ICosmosHelper cosmosHelper;
        private Container container;
        public AccountService(IAuthHelper authHelpers, CosmosClient cosmosClient, ICosmosHelper cosmosHelper)
        {
            this.authHelpers = authHelpers;
            this.cosmosHelper = cosmosHelper;
            this.container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public ApiResponse<AccountCreateResponse> Create(string username, string password, string userType = "user", string displayName = "DisplayName", UserStatus userStatus = UserStatus.Active)
        {
            var passwordValidation = authHelpers.ValidatePassword(password);
            if (!String.IsNullOrEmpty(passwordValidation))
            {
                return new ApiResponse<AccountCreateResponse> { error = passwordValidation, status = HttpStatusCode.BadRequest };
            }
            var salt = authHelpers.GetSalt();
            var user = new Models.User
            {
                id = Guid.NewGuid().ToString(),
                email = username,
                displayName = displayName,
                userStatus = UserStatus.Active,
                userType = userType,
                country = "US",
                hash = authHelpers.GetHashedString(password, salt),
                salt = salt,
            };
            try
            {
                ItemResponse<Models.User> response = container.CreateItemAsync<Models.User>(
                    item: user,
                    partitionKey: new PartitionKey(user.userType)
                ).Result;
                return new ApiResponse<AccountCreateResponse> { data = new AccountCreateResponse { id = user.id }, status = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Conflict (409)", StringComparison.OrdinalIgnoreCase))
                {
                    return new ApiResponse<AccountCreateResponse> { error = "Email conflict", status = HttpStatusCode.Conflict };
                }
            }
            return new ApiResponse<AccountCreateResponse> { error = "Unknown Error", status = HttpStatusCode.BadRequest };
        }

        public ApiResponse<AccountLogonResponse> Logon(AccountRequest accountRequest)
        {
            if (String.IsNullOrEmpty(accountRequest.username) || String.IsNullOrEmpty(accountRequest.password))
            {
                return new ApiResponse<AccountLogonResponse> { error = "Username and password required", status = HttpStatusCode.BadRequest };
            }
            var query = new QueryDefinition(query: "SELECT TOP 1 * FROM c WHERE c.email=@email")
                .WithParameter("@email", accountRequest.username);
            var user = cosmosHelper.GetFirst<Models.User>(container, query);
            if (user == null || String.IsNullOrEmpty(user.salt))
            {
                return new ApiResponse<AccountLogonResponse> { error = "Invalid Logon", status = HttpStatusCode.BadRequest };
            }
            var hash = authHelpers.GetHashedString(accountRequest.password, user.salt);
            var matches = hash.Equals(user.hash);
            if (matches)
            {
                if (user.userStatus != UserStatus.Active)
                {
                    return new ApiResponse<AccountLogonResponse> { error = "Account not active", status = HttpStatusCode.BadRequest };
                }
                var token = authHelpers.GenerateJWTToken(accountRequest.username, "");
                return new ApiResponse<AccountLogonResponse> { data = new AccountLogonResponse { token = token }, status = HttpStatusCode.OK };
            }
            return new ApiResponse<AccountLogonResponse> { error = "Invalid Logon", status = HttpStatusCode.BadRequest };
        }

        public ApiResponse<SetStatusChange> SetStatus(SetStatusChange setStatusRequest)
        {
            List<PatchOperation> patchOperations = new List<PatchOperation>()
            {
                PatchOperation.Replace("/userStatus", setStatusRequest.status),
            };
            ItemResponse<dynamic> item = this.container.PatchItemAsync<dynamic>(setStatusRequest.id, new PartitionKey(setStatusRequest.country), patchOperations).Result;
            if (item.StatusCode == HttpStatusCode.OK)
            {
                return new ApiResponse<SetStatusChange> { data = setStatusRequest, status = HttpStatusCode.OK };
            }
            return new ApiResponse<SetStatusChange> { error = "Bad Request", status = HttpStatusCode.BadRequest };
        }

        public ApiResponse<SpecificUser> Delete(SpecificUser specificUser)
        {
            ItemResponse<dynamic> item = this.container.DeleteItemAsync<dynamic>(specificUser.id, new PartitionKey(specificUser.country)).Result;
            if (item.StatusCode == HttpStatusCode.NoContent)
            {
                return new ApiResponse<SpecificUser> { data = specificUser, status = HttpStatusCode.OK };
            }
            return new ApiResponse<SpecificUser> { error = "Bad Request", status = HttpStatusCode.BadRequest };
        }
    }
}
