
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Formats.BusinessCard;
using GroupDocs.Metadata.Formats.Video;
using GroupDocs.Metadata.Standards.Exif;
using GroupDocs.Total.MVC.Products.Metadata.Repositories.Matroska;

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
                    return new OneLevelMetadataRepository(branchPackage);
                case MetadataType.Iptc:
                    return new IptcMetadataRepository(branchPackage);
                case MetadataType.ID3V2:
                    return new ID3V2TagRepository(branchPackage);
                case MetadataType.Matroska:
                    if (branchPackage is MatroskaTag)
                    {
                        return new MatroskaTagRepository(branchPackage);
                    }
                    if (branchPackage is MatroskaTrack)
                    {
                        return new MatroskaTrackRepository(branchPackage);
                    }
                    return new OneLevelMetadataRepository(branchPackage);
                case MetadataType.FileFormat:
                    return new FileTypeRepository(branchPackage);
                case MetadataType.VCard:
                    if (branchPackage is VCardCard)
                    {
                        return new VCardRepository(branchPackage);
                    }
                    return new OneLevelMetadataRepository(branchPackage);
                default:
                    return new OneLevelMetadataRepository(branchPackage);
            }
        }
    }
}