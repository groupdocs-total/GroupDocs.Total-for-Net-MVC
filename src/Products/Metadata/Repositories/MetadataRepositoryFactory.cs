
using GroupDocs.Metadata.Common;

namespace GroupDocs.Total.MVC.Products.Metadata.Repositories
{
    public static class MetadataRepositoryFactory
    {
        public static MetadataPackageRepository Create(MetadataPackage branchPackage)
        {
            switch (branchPackage.MetadataType)
            {
                case MetadataType.Xmp:
                    return new XmpMetadataRepository(branchPackage);
                case MetadataType.Exif:
                    return new ExifMetadataRepository(branchPackage);
                case MetadataType.Iptc:
                    return new IptcMetadataRepository(branchPackage);
                default:
                    return new OneLevelMetadataRepository(branchPackage);
            }
        }
    }
}