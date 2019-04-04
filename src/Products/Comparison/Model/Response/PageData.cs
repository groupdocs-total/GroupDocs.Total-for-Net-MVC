using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Comparison.Model.Response
{
    public class PageData
    {
        [JsonProperty]
        public string data;

        [JsonProperty]
        public int pageHeight;
    }
}