using Microsoft.Azure.Cosmos;

namespace JwtAuthApp.Helpers
{
    public interface ICosmosHelper
    {
        T? GetFirst<T>(Container container, QueryDefinition query) where T : class;
    }
    public class CosmosHelper : ICosmosHelper
    {
        public T? GetFirst<T>(Container container, QueryDefinition query) where T : class
        {
            using FeedIterator<T> feed = container.GetItemQueryIterator<T>(
                queryDefinition: query
            );
            while (feed.HasMoreResults)
            {
                FeedResponse<T> response = feed.ReadNextAsync().Result;
                foreach (T item in response)
                {
                    return item;
                }
                break;
            }
            return null;
        }

    }
}
