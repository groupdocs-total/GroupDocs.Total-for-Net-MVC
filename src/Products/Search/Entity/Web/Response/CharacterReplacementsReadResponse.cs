using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    internal class CharacterReplacementsReadResponse
    {
        [JsonProperty]
        public int[] Replacements { get; set; }
    }
}
