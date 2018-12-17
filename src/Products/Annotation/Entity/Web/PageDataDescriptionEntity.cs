using GroupDocs.Total.MVC.Products.Common.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Annotation.Entity.Web
{
    public class PageDataDescriptionEntity : PageDescriptionEntity
    {
        /// Annotation data    
        public string data;
        
        /// List of annotation data        
        public AnnotationDataEntity[] annotations;
    }
}