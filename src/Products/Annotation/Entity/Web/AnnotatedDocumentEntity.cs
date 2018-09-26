using GroupDocs.Total.MVC.Products.Common.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Annotation.Entity.Web
{
    public class AnnotatedDocumentEntity : DocumentDescriptionEntity
    {
        public string guid;
        public AnnotationDataEntity[] annotations;
    }
}