using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class FilesDeleteRequest : SearchBaseRequest
    {
        [JsonProperty]
        public PostedDataEntity[] Files { get; set; }
    }
}
