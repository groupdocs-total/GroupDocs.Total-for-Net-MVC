using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Common.Entity.Web.Request
{
    public class HighlightTermsRequest
    {
        [JsonProperty]
        internal string Html { get; set; }

        [JsonProperty]
        internal string[] Terms { get; set; }

        [JsonProperty]
        internal bool CaseSensitive { get; set; }
    }
}
