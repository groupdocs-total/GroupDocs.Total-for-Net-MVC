using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class StopWordsUpdateRequest
    {
        [JsonProperty]
        public string[] StopWords { get; set; }
    }
}
