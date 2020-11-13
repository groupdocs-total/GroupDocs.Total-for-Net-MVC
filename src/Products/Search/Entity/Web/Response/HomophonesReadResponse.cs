using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    public class HomophonesReadResponse
    {
        [JsonProperty]
        public string[][] HomophoneGroups { get; set; }
    }
}
