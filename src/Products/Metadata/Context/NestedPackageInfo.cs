

using GroupDocs.Metadata.Common;

namespace GroupDocs.Total.MVC.Products.Metadata.Context
{
    public class NestedPackageInfo
    {
        public NestedPackageInfo(MetadataPackage package, string path, int index = -1)
        {
            Package = package;
            Path = path;
            Index = index;
        }

        public MetadataPackage Package { get; private set; }

        public string Path { get; private set; }

        public int Index { get; private set; }
    }
}