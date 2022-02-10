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
    public class CreateProduct
    {
        private readonly IProductsRepository _repo;
        public CreateProduct(IProductsRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("CreateProduct")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "create")] HttpRequest req)
        {
            IActionResult result;

            try
            {
                using var read = new StreamReader(req.Body, Encoding.UTF8);
                var incomingReq = await read.ReadToEndAsync();

                if(!string.IsNullOrEmpty(incomingReq))
                {
                    var productReq = JsonConvert.DeserializeObject<Product>(incomingReq);
                    var product = new Product {
                        Id = Guid.NewGuid().ToString(),
                        Name = productReq.Name,
                        Image = productReq.Image,
                        Description = productReq.Description
                    };

                    await _repo.CreateProductAsync(product);
                    // result = new StatusCodeResult(StatusCodes.Status201Created);
                    result = new CreatedAtRouteResult(nameof(CreateProduct), new { id = product.Id}, product);
                }else
                {
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }


            }
            catch (System.Exception ex)
            {
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw new Exception($"Coud not create product! Message: {ex.Message}");
            }
            return result;
        }
    }
}
