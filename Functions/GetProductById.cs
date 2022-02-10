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
    public class GetProductById
    {
        private readonly IProductsRepository _repo;
        public GetProductById(IProductsRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("GetProductById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}")] HttpRequest req, string id)
        {
            IActionResult result = null;
            try
            {
                var product = await _repo.GetProductByIdAsync(id);

                if(product is null)
                {
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }else
                {
                    result = new OkObjectResult(product);
                }

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
