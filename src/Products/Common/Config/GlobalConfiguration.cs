using GroupDocs.Total.MVC.Products.Annotation.Config;
using GroupDocs.Total.MVC.Products.Signature.Config;
using GroupDocs.Total.MVC.Products.Viewer.Config;
using GroupDocs.Total.MVC.Products.Comparison.Config;
using GroupDocs.Total.MVC.Products.Conversion.Config;
using GroupDocs.Total.MVC.Products.Editor.Config;

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
        private readonly ConversionConfiguration Conversion;
        private readonly EditorConfiguration Editor;

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
            Conversion = new ConversionConfiguration();
            Editor = new EditorConfiguration();
        }

        public ConversionConfiguration GetConversionConfiguration() {
            return Conversion;
        }

        public EditorConfiguration GetEditorConfiguration()
        {
            return Editor;
        }
    }
}