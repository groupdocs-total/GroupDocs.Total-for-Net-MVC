using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Annotation.Config
{
    /// <summary>
    /// SignatureConfiguration
    /// </summary>
    public class AnnotationConfiguration : ConfigurationSection
    {
        public string FilesDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public string DefaultDocument { get; set; }        
        public int PreloadPageCount { get; set; }
        public bool isTextAnnotation { get; set; }
        public bool isAreaAnnotation { get; set;}
        public bool isPointAnnotation { get; set;}
        public bool isTextStrikeoutAnnotation { get; set;}
        public bool isPolylineAnnotation { get; set;}
        public bool isTextFieldAnnotation { get; set;}
        public bool isWatermarkAnnotation { get; set;}
        public bool isTextReplacementAnnotation { get; set;}
        public bool isArrowAnnotation { get; set;}
        public bool isTextRedactionAnnotation { get; set;}
        public bool isResourcesRedactionAnnotation { get; set;}
        public bool isTextUnderlineAnnotation { get; set;}
        public bool isDistanceAnnotation {get; set;}
        public bool isDownloadOriginal {get; set;}
        public bool isDownloadAnnotated {get; set;}
        private NameValueCollection annotationConfiguration = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("annotationConfiguration");

        /// <summary>
        /// Get annotation configuration section from the Web.config
        /// </summary>
        public AnnotationConfiguration()
        {
            FilesDirectory = annotationConfiguration["filesDirectory"];           
            if (!IsFullPath(FilesDirectory))
            {
                FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, annotationConfiguration["filesDirectory"]);
                if (!Directory.Exists(FilesDirectory))
                {
                    FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DocumentSamples");
                    Directory.CreateDirectory(FilesDirectory);
                }
            }
            OutputDirectory = annotationConfiguration["outputDirectory"];          
            isTextAnnotation = Convert.ToBoolean(annotationConfiguration["isTextAnnotation"]);
            isAreaAnnotation = Convert.ToBoolean(annotationConfiguration["isAreaAnnotation"]);
            isPointAnnotation = Convert.ToBoolean(annotationConfiguration["isPointAnnotation"]);
            isTextStrikeoutAnnotation = Convert.ToBoolean(annotationConfiguration["isTextStrikeoutAnnotation"]);
            isPolylineAnnotation = Convert.ToBoolean(annotationConfiguration["isPolylineAnnotation"]);
            isTextFieldAnnotation = Convert.ToBoolean(annotationConfiguration["isTextFieldAnnotation"]);
            isWatermarkAnnotation = Convert.ToBoolean(annotationConfiguration["isWatermarkAnnotation"]);
            isTextReplacementAnnotation = Convert.ToBoolean(annotationConfiguration["isTextReplacementAnnotation"]);
            isArrowAnnotation = Convert.ToBoolean(annotationConfiguration["isArrowAnnotation"]);
            isTextRedactionAnnotation = Convert.ToBoolean(annotationConfiguration["isTextRedactionAnnotation"]);
            isResourcesRedactionAnnotation = Convert.ToBoolean(annotationConfiguration["isResourcesRedactionAnnotation"]);
            isTextUnderlineAnnotation = Convert.ToBoolean(annotationConfiguration["isTextUnderlineAnnotation"]);
            isDistanceAnnotation = Convert.ToBoolean(annotationConfiguration["isDistanceAnnotation"]);
            isDownloadOriginal = Convert.ToBoolean(annotationConfiguration["isDownloadOriginal"]);
            isDownloadAnnotated = Convert.ToBoolean(annotationConfiguration["isDownloadAnnotated"]);
            PreloadPageCount = Convert.ToInt32(annotationConfiguration["preloadPageCount"]);
        }

        private static bool IsFullPath(string path)
        {
            return !String.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
    }
}