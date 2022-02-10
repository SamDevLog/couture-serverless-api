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
    public class UpdateProduct
    {
        private readonly IProductsRepository _repo;
        public UpdateProduct(IProductsRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("UpdateProduct")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id}")] HttpRequest req,
            string id)
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
                    using var r = new StreamReader(req.Body, Encoding.UTF8);
                    var incomingReq = await r.ReadToEndAsync();

                    if(!string.IsNullOrEmpty(incomingReq))
                    {
                        var productReq = JsonConvert.DeserializeObject<Product>(incomingReq);
                        var updatedProduct = new Product {
                                Id = id,
                                Name = productReq.Name,
                                Image = productReq.Image,
                                Description = productReq.Description
                        };

                        await _repo.UpdateProductAsync(id, updatedProduct);
                        result = new OkObjectResult(updatedProduct);
                    }else{
                        result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                    }
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
