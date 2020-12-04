using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class AlphabetUpdateRequest
    {
        [JsonProperty]
        public AlphabetCharacter[] Characters { get; set; }
    }
}
