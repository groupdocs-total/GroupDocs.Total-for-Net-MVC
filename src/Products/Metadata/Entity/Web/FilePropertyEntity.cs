using GroupDocs.Metadata.Common;

namespace GroupDocs.Total.MVC.Products.Metadata.Entity.Web
{
    /// <summary>
    /// File property entity
    /// </summary>
    public class FilePropertyEntity
    {
        public string name { get; set; }

        public dynamic value { get; set; }

        public MetadataPropertyType type { get; set; }
    }
}