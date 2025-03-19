using JwtAuthApp.Helpers;
using Microsoft.Azure.Cosmos;

namespace JwtAuthApp.Services
{
    public interface IUserService
    {
        ApiResponse<Models.User> GetById(string id);
        ApiResponse<IEnumerable<Models.User>> Get(int skip, int take, string? search);
    }
    public class UserService : IUserService
    {
        private const string databaseName = "bty0Db";
        private const string containerName = "user";

        private Container container;
        private ICosmosHelper cosmosHelper;

        public UserService(CosmosClient cosmosClient, ICosmosHelper cosmosHelper)
        {
            this.container = cosmosClient.GetContainer(databaseName, containerName);
            this.cosmosHelper = cosmosHelper;
        }

        public ApiResponse<Models.User> GetById(string id)
        {
            var query = new QueryDefinition(
                query: "SELECT TOP 1 c.id,c.email,c.userType FROM c WHERE IS_NULL(@id) or c.id=@id ORDER BY c.email")
                .WithParameter("@id", id);
            var user = cosmosHelper.GetFirst<Models.User>(container, query);
            return new ApiResponse<Models.User> { data = user, status = System.Net.HttpStatusCode.OK };
        }

        public ApiResponse<IEnumerable<Models.User>> Get(int skip, int take, string? search)
        {
            var query = new QueryDefinition(
                query: "SELECT c.id, c.email, c.userStatus, c.userType, c.displayName FROM c WHERE IS_NULL(@search) OR c.email LIKE @search ORDER BY c.email OFFSET @skip LIMIT @take")
                .WithParameter("@skip", skip)
                .WithParameter("@take", take)
                .WithParameter("@search", "%" + search + "%");
            using FeedIterator<Models.User> feed = container.GetItemQueryIterator<Models.User>(
                queryDefinition: query
            );
            var users = new List<Models.User>();
            while (feed.HasMoreResults)
            {
                FeedResponse<Models.User> response = feed.ReadNextAsync().Result;
                foreach (Models.User item in response)
                {
                    users.Add(item);
                }
            }
            return new ApiResponse<IEnumerable<Models.User>> { data = users, status = System.Net.HttpStatusCode.OK };
        }
    }
}
