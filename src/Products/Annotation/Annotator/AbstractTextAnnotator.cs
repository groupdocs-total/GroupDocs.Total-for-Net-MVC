using GroupDocs.Annotation.Domain;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public abstract class AbstractTextAnnotator : BaseAnnotator
    {
        protected AbstractTextAnnotator(AnnotationDataEntity annotationData, PageData pageData)
            : base(annotationData, pageData)
        {
        }

        protected new AnnotationInfo InitAnnotationInfo()
        {
            AnnotationInfo annotationInfo = base.InitAnnotationInfo();
            annotationInfo.FieldText = annotationData.text;
            annotationInfo.FontFamily = annotationData.font;
            annotationInfo.FontSize = annotationData.fontSize;
            annotationInfo.FontColor = annotationData.fontColor;
            return annotationInfo;
        }

        protected override Rectangle GetBox()
        {
            return new Rectangle(annotationData.left, annotationData.top, annotationData.width, annotationData.height);
        }
    }
}