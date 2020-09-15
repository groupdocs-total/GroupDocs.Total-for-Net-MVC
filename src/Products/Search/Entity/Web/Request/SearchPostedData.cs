using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class SearchPostedData : PostedDataEntity
    {
        [JsonProperty()]
        internal string Query { get; set; }

        [JsonProperty()]
        internal bool CaseSensitiveSearch { get; set; }

        [JsonProperty()]
        internal bool FuzzySearch { get; set; }

        [JsonProperty()]
        internal int FuzzySearchMistakeCount { get; set; }

        [JsonProperty()]
        internal bool FuzzySearchOnlyBestResults { get; set; }

        [JsonProperty()]
        internal bool KeyboardLayoutCorrection { get; set; }

        [JsonProperty()]
        internal bool SynonymSearch { get; set; }

        [JsonProperty()]
        internal bool HomophoneSearch { get; set; }

        [JsonProperty()]
        internal bool WordFormsSearch { get; set; }

        [JsonProperty()]
        internal bool SpellingCorrection { get; set; }

        [JsonProperty()]
        internal int SpellingCorrectionMistakeCount { get; set; }

        [JsonProperty()]
        internal bool SpellingCorrectionOnlyBestResults { get; set; }
    }
}
