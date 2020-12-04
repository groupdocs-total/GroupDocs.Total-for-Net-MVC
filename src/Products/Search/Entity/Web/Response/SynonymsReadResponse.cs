using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    public class SynonymsReadResponse
    {
        [JsonProperty]
        public string[][] SynonymGroups { get; set; }
    }
}
