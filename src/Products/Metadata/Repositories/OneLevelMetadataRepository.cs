
using GroupDocs.Metadata.Common;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.Repositories
{
    public class OneLevelMetadataRepository : MetadataPackageRepository
    {
        public OneLevelMetadataRepository(MetadataPackage package) : base(package)
        {
        }

        protected override IEnumerable<MetadataPackage> GetNestedPackages()
        {
            yield return Package;
        }
    }
}