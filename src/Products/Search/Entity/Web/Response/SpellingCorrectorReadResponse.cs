using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    internal class SpellingCorrectorReadResponse
    {
        [JsonProperty]
        public string[] Words { get; set; }
    }
}
