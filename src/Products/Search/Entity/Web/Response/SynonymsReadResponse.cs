using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    internal class SynonymsReadResponse
    {
        [JsonProperty]
        public string[][] SynonymGroups { get; set; }
    }
}
