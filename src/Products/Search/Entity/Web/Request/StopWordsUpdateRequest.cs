using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class StopWordsUpdateRequest
    {
        [JsonProperty]
        internal string[] StopWords { get; set; }
    }
}
