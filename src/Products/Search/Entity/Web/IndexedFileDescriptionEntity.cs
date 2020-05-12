using GroupDocs.Search.Common;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web
{
    public class IndexedFileDescriptionEntity : FileDescriptionEntity
    {
        public DocumentStatus documentStatus { get; set; }
    }
}