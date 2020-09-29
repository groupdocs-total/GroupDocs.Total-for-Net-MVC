using GroupDocs.Total.MVC.Products.Search.Entity.Web.Response;
using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class AlphabetUpdateRequest
    {
        [JsonProperty]
        internal AlphabetCharacter[] Characters { get; set; }
    }
}
