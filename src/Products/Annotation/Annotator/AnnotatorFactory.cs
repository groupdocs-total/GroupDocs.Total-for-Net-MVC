
using GroupDocs.Annotation.Domain;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using System;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class AnnotatorFactory
    {
        /// <summary>
        /// Create annotator instance depending on type of annotation
        /// </summary>
        /// <param name="annotationData">AnnotationDataEntity</param>
        /// <param name="pageData">PageData</param>
        /// <returns></returns>
        public static BaseAnnotator createAnnotator(AnnotationDataEntity annotationData, PageData pageData)
        {
            annotationData = RoundCoordinates(annotationData);
            switch (annotationData.type)
            {
                case "text":                    
                    return new TextAnnotator(annotationData, pageData);
                case "area":
                    return new AreaAnnotator(annotationData, pageData);
                case "point":
                    return new PointAnnotator(annotationData, pageData);
                case "textStrikeout":
                    return new TexStrikeoutAnnotator(annotationData, pageData);
                case "polyline":
                    return new PolylineAnnotator(annotationData, pageData);
                case "textField":
                    return new TextFieldAnnotator(annotationData, pageData);
                case "watermark":
                    return new WatermarkAnnotator(annotationData, pageData);
                case "textReplacement":
                    return new TextReplacementAnnotator(annotationData, pageData);
                case "arrow":
                    return new ArrowAnnotator(annotationData, pageData);
                case "textRedaction":
                    return new TextRedactionAnnotator(annotationData, pageData);
                case "resourcesRedaction":
                    return new ResourceRedactionAnnotator(annotationData, pageData);
                case "textUnderline":
                    return new TexUnderlineAnnotator(annotationData, pageData);
                case "distance":
                    return new DistanceAnnotator(annotationData, pageData);
                default:
                    throw new ArgumentNullException("Wrong annotation data without annotation type!");
            }
        }

        private static AnnotationDataEntity RoundCoordinates(AnnotationDataEntity annotationData)
        {
            annotationData.height = (float)Math.Round(annotationData.height, 0);
            annotationData.left = (float)Math.Round(annotationData.left, 0);
            annotationData.top = (float)Math.Round(annotationData.top, 0);
            annotationData.width = (float)Math.Round(annotationData.width, 0);
            return annotationData;
        }
    }
}