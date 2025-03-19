using JwtAuthApp.Helpers;
using JwtAuthApp.Models;
using Microsoft.Azure.Cosmos;
using System.IO;

namespace JwtAuthApp.Data
{
    public class DataLoader
    {

        internal static void LoadLanguages(WebApplication app)
        {
            var databaseName = "bty0db";
            var containerName = "language";
            var partitionName = "/set1";
            var client = app.Services.GetRequiredService<CosmosClient>();

            var database = client.GetDatabase(databaseName);

            database.CreateContainerIfNotExistsAsync(containerName, partitionName).Wait();
            var container = client.GetContainer(databaseName, containerName);

            using (StreamReader reader = new StreamReader("Data/language.tsv"))
            {
                while (!reader.EndOfStream)
                {
                    var parts = reader.ReadLine().Split('\t');
                    ItemResponse<Models.Language> item = container.UpsertItemAsync<Models.Language>(new Models.Language
                    {
                        id = parts[0],
                        name = parts[1],
                        set1 = parts[2],
                        set2 = parts[3],
                        scope = parts[4],
                        type = parts[5],
                        endonym = parts[6],
                    }, new PartitionKey(parts[2])).Result;
                }
            }
        }

        internal static void LoadCountries(WebApplication app)
        {
            var databaseName = "bty0db";
            var containerName = "country";
            var partitionName = "/a2";
            var client = app.Services.GetRequiredService<CosmosClient>();

            var database = client.GetDatabase(databaseName);

            database.CreateContainerIfNotExistsAsync(containerName, partitionName).Wait();
            var container = client.GetContainer(databaseName, containerName);

            using (StreamReader reader = new StreamReader("Data/country.tsv"))
            {
                while (!reader.EndOfStream)
                {
                    var parts = reader.ReadLine().Split('\t');
                    ItemResponse<Models.Country> item = container.UpsertItemAsync<Models.Country>(new Models.Country
                    {
                        id = parts[0],
                        name = parts[1],
                        officialName = parts[2],
                        sovereignty = parts[3],
                        a2 = parts[4],
                        a3 = parts[5],
                        number = int.Parse(parts[6]),
                        tld = parts[7]
                    }, new PartitionKey(parts[4])).Result;
                }
            }
        }

    }
}
