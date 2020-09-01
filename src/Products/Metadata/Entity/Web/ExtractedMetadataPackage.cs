

using GroupDocs.Metadata.Common;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.Entity.Web
{
    public class ExtractedMetadataPackage
    {
        public string id { get; set; }

        public string name { get; set; }

        public MetadataType type { get; set; }

        public List<FilePropertyEntity> properties { get; set; }

        public List<FilePropertyName> knownProperties { get; set; }
    }
}