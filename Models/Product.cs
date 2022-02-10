using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AzureFunctionsInProcess.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
    }
}