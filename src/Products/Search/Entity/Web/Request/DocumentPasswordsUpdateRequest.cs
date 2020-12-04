using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class DocumentPasswordsUpdateRequest
    {
        [JsonProperty]
        public KeyPasswordPair[] Passwords { get; set; }
    }
}
