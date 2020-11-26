using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class CharacterReplacementsUpdateRequest
    {
        [JsonProperty]
        public int[] Replacements { get; set; }
    }
}
