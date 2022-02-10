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
using System.Text;
using AzureFunctionsInProcess.Models;

namespace Shop
{
    public class DeleteProduct
    {
        private readonly IProductsRepository _repo;

        public DeleteProduct(IProductsRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("DeleteProduct")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id}")] HttpRequest req, string id)
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
                    await _repo.DeleteProductAsync(product);
                    result = new StatusCodeResult(StatusCodes.Status204NoContent);
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
