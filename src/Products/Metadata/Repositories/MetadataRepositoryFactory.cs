
using GroupDocs.Metadata.Common;

namespace GroupDocs.Total.MVC.Products.Metadata.Repositories
{
    public static class MetadataRepositoryFactory
    {
        public static MetadataPackageRepository Create(MetadataPackage package)
        {
            switch (package.MetadataType)
            {
                case MetadataType.Xmp:
                    return new XmpMetadataRepository(package);
                case MetadataType.Exif:
                    return new ExifMetadataRepository(package);
                default:
                    return new OneLevelMetadataRepository(package);
            }
        }
    }
}