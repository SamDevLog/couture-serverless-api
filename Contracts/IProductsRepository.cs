using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctionsInProcess.Models;
using Microsoft.Azure.Cosmos.Scripts;

namespace AzureFunctionsInProcess.Contracts
{
    public interface IProductsRepository
    {
        Task CreateDatabaseAsync();
        Task CreateContainerAsync();
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(string id);
        Task CreateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<Product> UpdateProductAsync(string id, Product product);    
    }
}