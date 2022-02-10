using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctionsInProcess.Contracts;

namespace Shop
{
    public class GetProducts
    {
        private readonly IProductsRepository _repo;
        public GetProducts(IProductsRepository repo)
        {
            _repo = repo;
        }
        
        [FunctionName("GetProducts")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequest req)
        {
            IActionResult result = null;
            try
            {
                var products = await _repo.GetAllProductsAsync();
                if(products is null)
                {
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }

                result = new OkObjectResult(products);
            }
            catch (System.Exception ex)
            {
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
