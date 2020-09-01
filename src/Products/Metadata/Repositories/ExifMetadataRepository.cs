
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Standards.Exif;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.Repositories
{
    public class ExifMetadataRepository : MetadataPackageRepository
    {
        public ExifMetadataRepository(MetadataPackage package) : base(package)
        {
        }

        protected override IEnumerable<MetadataPackage> GetNestedPackages()
        {
            var exifPackage = (ExifPackage)Package;
            yield return exifPackage.ExifIfdPackage;
            yield return exifPackage.GpsPackage;
            yield return exifPackage;
        }
    }
}