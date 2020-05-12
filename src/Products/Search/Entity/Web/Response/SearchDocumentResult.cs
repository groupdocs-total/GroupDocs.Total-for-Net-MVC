using GroupDocs.Search.Results;
using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    public class SearchDocumentResult
    {
        [JsonProperty]
        private FoundDocumentField[] foundFields { get; set; }

        [JsonProperty]
        private string guid { get; set; }

        [JsonProperty]
        private string name { get; set; }

        [JsonProperty]
        private long size { get; set; }

        [JsonProperty]
        private int occurrences { get; set; }

        [JsonProperty]
        private string[] foundPhrases { get; set; }

        public FoundDocumentField[] GetFoundFields()
        {
            return this.foundFields;
        }

        public void SetFoundFields(FoundDocumentField[] foundFields)
        {
            this.foundFields = foundFields;
        }

        public string GetGuid()
        {
            return this.guid;
        }

        public void SetGuid(string guid)
        {
            this.guid = guid;
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public long GetSize()
        {
            return this.size;
        }

        public void SetSize(long size)
        {
            this.size = size;
        }

        public int GetOccurrences()
        {
            return this.occurrences;
        }

        public void SetOccurrences(int occurences)
        {
            this.occurrences = occurences;
        }

        public string[] GetFoundPhrases()
        {
            return this.foundPhrases;
        }

        public void SetFoundPhrases(string[] foundPhrases)
        {
            this.foundPhrases = foundPhrases;
        }
    }
}