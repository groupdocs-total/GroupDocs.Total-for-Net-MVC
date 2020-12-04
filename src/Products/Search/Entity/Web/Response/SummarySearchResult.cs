using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    public class SummarySearchResult
    {
        [JsonProperty]
        public int totalOccurences { get; set; }

        [JsonProperty]
        public int totalFiles { get; set; }

        [JsonProperty]
        public int indexedFiles { get; set; }

        [JsonProperty]
        public SearchDocumentResult[] foundFiles { get; set; }

        [JsonProperty]
        public string searchDuration { get; set; }
    }
}
