using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Annotation.Entity.Web
{
    public class AnnotatedDocumentEntity : DocumentDescriptionEntity
    {
        public string guid;
        public AnnotationDataEntity[] annotations;
        public string[] supportedAnnotations;
    }
}