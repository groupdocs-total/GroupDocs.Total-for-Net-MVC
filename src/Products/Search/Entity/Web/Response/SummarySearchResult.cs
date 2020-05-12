using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    public class SummarySearchResult
    {
        [JsonProperty]
        private int totalOccurences { get; set; }

        [JsonProperty]
        private int totalFiles { get; set; }

        [JsonProperty]
        private int indexedFiles { get; set; }

        [JsonProperty]
        private SearchDocumentResult[] foundFiles { get; set; }

        [JsonProperty]
        private string searchDuration { get; set; }

        public int GetTotalOccurences()
        {
            return this.totalOccurences;
        }

        public void SetTotalOccurences(int totalOccurences)
        {
            this.totalOccurences = totalOccurences;
        }

        public int GetTotalFiles()
        {
            return this.totalFiles;
        }

        public void SetTotalFiles(int totalFiles)
        {
            this.totalFiles = totalFiles;
        }

        public int GetIndexedFiles()
        {
            return this.indexedFiles;
        }

        public void SetIndexedFiles(int indexedFiles)
        {
            this.indexedFiles = indexedFiles;
        }

        public SearchDocumentResult[] GetFoundFiles()
        {
            return this.foundFiles;
        }

        public void SetFoundFiles(SearchDocumentResult[] foundFiles)
        {
            this.foundFiles = foundFiles;
        }

        public string GetSearchDuration()
        {
            return this.searchDuration;
        }

        public void SetSearchDuration(string searchDuration)
        {
            this.searchDuration = searchDuration;
        }
    }
}