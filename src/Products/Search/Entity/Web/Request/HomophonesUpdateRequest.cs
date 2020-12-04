using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class HomophonesUpdateRequest
    {
        [JsonProperty]
        public string[][] HomophoneGroups { get; set; }
    }
}
