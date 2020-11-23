using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class SpellingCorrectorUpdateRequest
    {
        [JsonProperty]
        public string[] Words { get; set; }
    }
}
