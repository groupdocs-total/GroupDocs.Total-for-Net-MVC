using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class SynonymsUpdateRequest
    {
        [JsonProperty]
        internal string[][] SynonymGroups { get; set; }
    }
}
