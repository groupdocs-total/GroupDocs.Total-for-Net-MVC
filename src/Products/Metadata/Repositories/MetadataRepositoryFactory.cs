
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Standards.Exif;

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
                    if (branchPackage is ExifPackage)
                    {
                        return new ExifMetadataRepository(branchPackage);
                    }
                    else
                    {
                        return new OneLevelMetadataRepository(branchPackage);
                    }
                case MetadataType.Iptc:
                    return new IptcMetadataRepository(branchPackage);
                case MetadataType.ID3V2:
                    return new ID3V2TagRepository(branchPackage);
                default:
                    return new OneLevelMetadataRepository(branchPackage);
            }
        }
    }
}