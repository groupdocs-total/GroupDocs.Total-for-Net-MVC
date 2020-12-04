
using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    public class SearchDocumentResult
    {
        [JsonProperty]
        public string guid { get; set; }

        [JsonProperty]
        public string name { get; set; }

        [JsonProperty]
        public long size { get; set; }

        [JsonProperty]
        public int occurrences { get; set; }

        [JsonProperty]
        public string[] foundPhrases { get; set; }

        [JsonProperty]
        public string[] terms { get; set; }
    }
}
