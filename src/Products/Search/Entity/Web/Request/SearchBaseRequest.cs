using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class SearchBaseRequest
    {
        [JsonProperty]
        public string FolderName { get; set; }
    }
}
