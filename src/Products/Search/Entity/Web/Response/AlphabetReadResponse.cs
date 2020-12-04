using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    public class AlphabetReadResponse
    {
        [JsonProperty]
        public AlphabetCharacter[] Characters { get; set; }
    }
}
