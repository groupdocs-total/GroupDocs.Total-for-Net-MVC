using GroupDocs.Total.MVC.Products.Annotation.Config;
using GroupDocs.Total.MVC.Products.Signature.Config;
using GroupDocs.Total.MVC.Products.Viewer.Config;
using GroupDocs.Total.MVC.Products.Comparison.Config;
using GroupDocs.Total.MVC.Products.Metadata.Config;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    /// <summary>
    /// Global configuration
    /// </summary>
    public class GlobalConfiguration
    {
        public ServerConfiguration Server;
        public ApplicationConfiguration Application;
        public CommonConfiguration Common;
        public SignatureConfiguration Signature;
        public ViewerConfiguration Viewer;
        public AnnotationConfiguration Annotation;
        public ComparisonConfiguration Comparison;
        public MetadataConfiguration Metadata;

        /// <summary>
        /// Get all configurations
        /// </summary>
        public GlobalConfiguration()
        {
            Server = new ServerConfiguration();
            Application = new ApplicationConfiguration();
            Signature = new SignatureConfiguration();
            Viewer = new ViewerConfiguration();
            Common = new CommonConfiguration();
            Annotation = new AnnotationConfiguration();
            Comparison = new ComparisonConfiguration();
            Metadata = new MetadataConfiguration();
        }
    }
}