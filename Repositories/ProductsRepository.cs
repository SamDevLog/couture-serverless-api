using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctionsInProcess.Contracts;
using AzureFunctionsInProcess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace AzureFunctionsInProcess.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly IConfiguration _configuration;
        private CosmosClient _cosmosClient;
        private Database _database;
        private  Container _container;
        private string databaseId = string.Empty;
        private string containerId = string.Empty;
        public ProductsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionString = configuration["CosmosDbTest"];
            databaseId = "mustapha-db";
            containerId = "Products";

            _cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions(){
                ConnectionMode = ConnectionMode.Gateway
            });

            CreateDatabaseAsync().Wait();
            CreateContainerAsync().Wait();
        }

        public async Task CreateContainerAsync()
        {
            _container = await _database.CreateContainerIfNotExistsAsync(containerId, "/id");
        }

        public async Task CreateDatabaseAsync()
        {
            _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }

        public async Task CreateProductAsync(Product product)
        {
            try
            {
                ItemResponse<Product> itemResponse = await _container.ReadItemAsync<Product>(product.Id, new PartitionKey(product.Id));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _container.CreateItemAsync<Product>(product, new PartitionKey(product.Id));
            }
        }

        public async Task DeleteProductAsync(Product product)
        {
            var partitionKeyValue = product.Id;
            await _container.DeleteItemAsync<Product>(product.Id, new PartitionKey(partitionKeyValue));
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var qry = string.Format("SELECT * FROM c");

            QueryDefinition queryDefinition = new QueryDefinition(qry);
            FeedIterator<Product> queryIterator = _container.GetItemQueryIterator<Product>(queryDefinition);
            
            List<Product> products = new();

            while(queryIterator.HasMoreResults)
            {
                FeedResponse<Product> resultSet = await queryIterator.ReadNextAsync();
                foreach (var p in resultSet)
                {
                    products.Add(p);
                }
            }
            return products;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {

            try
            {
                var qry = string.Format("SELECT * FROM c WHERE c.id = '{0}'", id);
                QueryDefinition queryDefinition = new QueryDefinition(qry);
                FeedIterator<Product> queryIterator = _container.GetItemQueryIterator<Product>(queryDefinition);

                

                Product product = new();

                while(queryIterator.HasMoreResults)
                {
                    FeedResponse<Product> resultSet = await queryIterator.ReadNextAsync();
                    foreach (var p in resultSet)
                    {
                        product.Id = p.Id;
                        product.Name = p.Name;
                        product.Image = p.Image;
                        product.Description = p.Description;
                    }
                }

                if(product.Id is null) return null;
                
                return product;
            }
            catch (CosmosException ex)
            {
                throw new System.Exception($"Error Occured {ex.Message}");
            }
            
        }

        public async Task<Product> UpdateProductAsync(string id, Product product)
        {
            ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, new PartitionKey(id));

            var result = response.Resource;

            result.Id = product.Id;
            result.Name = product.Name;
            result.Image = product.Image;
            result.Description = product.Description;

            return await _container.ReplaceItemAsync<Product>(result, result.Id, new PartitionKey(id));
        }
    }
}