using GroupDocs.Metadata.Common;

namespace GroupDocs.Total.MVC.Products.Metadata.Entity.Web
{
    public class FilePropertyName
    {
        public string name { get; set; }

        public MetadataPropertyType type { get; set; }

        public PropertyAccessLevels accessLevel { get; set; }
    }
}