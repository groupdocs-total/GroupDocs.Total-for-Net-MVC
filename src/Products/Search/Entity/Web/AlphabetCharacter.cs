using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web
{
    public class AlphabetCharacter
    {
        [JsonProperty]
        public int Character { get; set; }

        [JsonProperty]
        public int Type { get; set; }
    }
}
