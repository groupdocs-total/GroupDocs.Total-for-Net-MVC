using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Annotation.Config
{
    /// <summary>
    /// SignatureConfiguration
    /// </summary>
    public class AnnotationConfiguration
    {
        public string FilesDirectory = "DocumentSamples";
        public string OutputDirectory = "";
        public string DefaultDocument = "";
        public int PreloadPageCount = 0;
        public bool isTextAnnotation = true;
        public bool isAreaAnnotation = true;
        public bool isPointAnnotation = true;
        public bool isTextStrikeoutAnnotation = true;
        public bool isPolylineAnnotation = true;
        public bool isTextFieldAnnotation = true;
        public bool isWatermarkAnnotation = true;
        public bool isTextReplacementAnnotation = true;
        public bool isArrowAnnotation = true;
        public bool isTextRedactionAnnotation = true;
        public bool isResourcesRedactionAnnotation = true;
        public bool isTextUnderlineAnnotation = true;
        public bool isDistanceAnnotation = true;
        public bool isDownloadOriginal = true;
        public bool isDownloadAnnotated = true;     

        /// <summary>
        /// Get annotation configuration section from the Web.config
        /// </summary>
        public AnnotationConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("annotation");
            // get Viewer configuration section from the web.config
            FilesDirectory = (configuration != null && !String.IsNullOrEmpty(configuration["filesDirectory"].ToString())) ? configuration["filesDirectory"] : FilesDirectory;
            if (!IsFullPath(FilesDirectory))
            {
                FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilesDirectory);
                if (!Directory.Exists(FilesDirectory))
                {                   
                    Directory.CreateDirectory(FilesDirectory);
                }
            }
            OutputDirectory = (configuration != null && !String.IsNullOrEmpty(configuration["outputDirectory"].ToString())) ? 
                configuration["outputDirectory"] : 
                OutputDirectory;
            isTextAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["textAnnotation"].ToString())) ? 
                Convert.ToBoolean(configuration["textAnnotation"]) : 
                isTextAnnotation;
            isAreaAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["areaAnnotation"].ToString())) ? 
                Convert.ToBoolean(configuration["areaAnnotation"]) : 
                isAreaAnnotation;
            isPointAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["pointAnnotation"].ToString())) ? 
                Convert.ToBoolean(configuration["pointAnnotation"]) : 
                isPointAnnotation;
            isTextStrikeoutAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["textStrikeoutAnnotation"].ToString())) ? 
                Convert.ToBoolean(configuration["textStrikeoutAnnotation"]) : 
                isTextStrikeoutAnnotation;
            isPolylineAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["polylineAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["polylineAnnotation"]) :
                isPolylineAnnotation;
            isTextFieldAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["textFieldAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["textFieldAnnotation"]) :
                isTextFieldAnnotation;
            isWatermarkAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["watermarkAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["watermarkAnnotation"]) :
                isWatermarkAnnotation;
            isTextReplacementAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["textReplacementAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["textReplacementAnnotation"]) :
                isTextReplacementAnnotation;
            isArrowAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["arrowAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["arrowAnnotation"]) :
                isArrowAnnotation;
            isTextRedactionAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["textRedactionAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["textRedactionAnnotation"]) :
                isTextRedactionAnnotation;
            isResourcesRedactionAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["resourcesRedactionAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["resourcesRedactionAnnotation"]) :
                isResourcesRedactionAnnotation;
            isTextUnderlineAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["textUnderlineAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["textUnderlineAnnotation"]) :
                isTextUnderlineAnnotation;
            isDistanceAnnotation = (configuration != null && !String.IsNullOrEmpty(configuration["distanceAnnotation"].ToString())) ?
                Convert.ToBoolean(configuration["distanceAnnotation"]) :
                isDistanceAnnotation;
            isDownloadOriginal = (configuration != null && !String.IsNullOrEmpty(configuration["downloadOriginal"].ToString())) ?
                Convert.ToBoolean(configuration["downloadOriginal"]) :
                isDownloadOriginal;
            isDownloadAnnotated = (configuration != null && !String.IsNullOrEmpty(configuration["downloadAnnotated"].ToString())) ?
                Convert.ToBoolean(configuration["downloadAnnotated"]) :
                isDownloadAnnotated;
            PreloadPageCount = (configuration != null && !String.IsNullOrEmpty(configuration["preloadPageCount"].ToString())) ?
                Convert.ToInt32(configuration["preloadPageCount"]) :
                PreloadPageCount;
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