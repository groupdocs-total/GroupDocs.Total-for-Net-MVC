using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class FileStatusGetRequest
    {
        [JsonProperty]
        public string FolderName { get; set; }

        [JsonProperty]
        public PostedDataEntity[] Files { get; set; }
    }
}
