using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AzureFunction.Portfolio
{
    public class Contact
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("msg")]
        public string? Msg { get; set; }
    }
}
