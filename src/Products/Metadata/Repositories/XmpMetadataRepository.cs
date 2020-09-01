
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Standards.Xmp;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.Repositories
{
    public class XmpMetadataRepository : MetadataPackageRepository
    {
        public XmpMetadataRepository(MetadataPackage package) : base(package)
        {
        }

        protected override IEnumerable<MetadataPackage> GetNestedPackages()
        {
            return ((XmpPacketWrapper)Package).Packages;
        }
    }
}