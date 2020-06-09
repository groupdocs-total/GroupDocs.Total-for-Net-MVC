using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Annotation.Config
{
    /// <summary>
    /// AnnotationConfiguration
    /// </summary>
    public class AnnotationConfiguration : CommonConfiguration
    {
        [JsonProperty]
        private string FilesDirectory = "DocumentSamples/Annotation";

        [JsonProperty]
        private string defaultDocument = "";

        [JsonProperty]
        private int preloadPageCount;

        [JsonProperty]
        private bool textAnnotation = true;

        [JsonProperty]
        private bool areaAnnotation = true;

        [JsonProperty]
        private bool pointAnnotation = true;

        [JsonProperty]
        private bool textStrikeoutAnnotation = true;

        [JsonProperty]
        private bool polylineAnnotation = true;

        [JsonProperty]
        private bool textFieldAnnotation = true;

        [JsonProperty]
        private bool watermarkAnnotation = true;

        [JsonProperty]
        private bool textReplacementAnnotation = true;

        [JsonProperty]
        private bool arrowAnnotation = true;

        [JsonProperty]
        private bool textRedactionAnnotation = true;

        [JsonProperty]
        private bool resourcesRedactionAnnotation = true;

        [JsonProperty]
        private bool textUnderlineAnnotation = true;

        [JsonProperty]
        private bool distanceAnnotation = true;

        [JsonProperty]
        private bool downloadOriginal = true;

        [JsonProperty]
        private bool downloadAnnotated = true;

        [JsonProperty]
        private bool zoom = true;

        [JsonProperty]
        private bool fitWidth = true;

        /// <summary>
        /// Get annotation configuration section from the Web.config
        /// </summary>
        public AnnotationConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("annotation");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);
            // get Viewer configuration section from the web.config
            FilesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", FilesDirectory);
            if (!IsFullPath(FilesDirectory))
            {
                FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilesDirectory);
                if (!Directory.Exists(FilesDirectory))
                {                   
                    Directory.CreateDirectory(FilesDirectory);
                }
            }
            defaultDocument = valuesGetter.GetStringPropertyValue("defaultDocument", defaultDocument).Replace(@"\", "/");
            textAnnotation = valuesGetter.GetBooleanPropertyValue("textAnnotation", textAnnotation);
            areaAnnotation = valuesGetter.GetBooleanPropertyValue("areaAnnotation", areaAnnotation);
            pointAnnotation = valuesGetter.GetBooleanPropertyValue("pointAnnotation", pointAnnotation);
            textStrikeoutAnnotation = valuesGetter.GetBooleanPropertyValue("textStrikeoutAnnotation", textStrikeoutAnnotation);
            polylineAnnotation = valuesGetter.GetBooleanPropertyValue("polylineAnnotation", polylineAnnotation);
            textFieldAnnotation = valuesGetter.GetBooleanPropertyValue("textFieldAnnotation", textFieldAnnotation);
            watermarkAnnotation = valuesGetter.GetBooleanPropertyValue("watermarkAnnotation", watermarkAnnotation);
            textReplacementAnnotation = valuesGetter.GetBooleanPropertyValue("textReplacementAnnotation", textReplacementAnnotation);
            arrowAnnotation = valuesGetter.GetBooleanPropertyValue("arrowAnnotation", arrowAnnotation);
            textRedactionAnnotation = valuesGetter.GetBooleanPropertyValue("textRedactionAnnotation", textRedactionAnnotation);
            resourcesRedactionAnnotation = valuesGetter.GetBooleanPropertyValue("resourcesRedactionAnnotation", resourcesRedactionAnnotation);
            textUnderlineAnnotation = valuesGetter.GetBooleanPropertyValue("textUnderlineAnnotation", textUnderlineAnnotation);
            distanceAnnotation = valuesGetter.GetBooleanPropertyValue("distanceAnnotation", distanceAnnotation);
            downloadOriginal = valuesGetter.GetBooleanPropertyValue("downloadOriginal", downloadOriginal);
            downloadAnnotated = valuesGetter.GetBooleanPropertyValue("downloadAnnotated", downloadAnnotated);
            preloadPageCount = valuesGetter.GetIntegerPropertyValue("preloadPageCount", preloadPageCount);
            zoom = valuesGetter.GetBooleanPropertyValue("zoom", zoom);
            fitWidth = valuesGetter.GetBooleanPropertyValue("fitWidth", fitWidth);
        }

        private static bool IsFullPath(string path)
        {
            return !String.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }

        public void SetFilesDirectory(string filesDirectory) {
            this.FilesDirectory = filesDirectory;
        }

        public string GetFilesDirectory()
        {
            return FilesDirectory;
        }       

        public void SetDefaultDocument(string defaultDocument)
        {
            this.defaultDocument = defaultDocument;
        }

        public string GetDefaultDocument()
        {
            return defaultDocument;
        }

        public void SetPreloadPageCount(int preloadPageCount)
        {
            this.preloadPageCount = preloadPageCount;
        }

        public int GetPreloadPageCount()
        {
            return preloadPageCount;
        }

        public void SetIsTextAnnotation(bool isTextAnnotation)
        {
            this.textAnnotation = isTextAnnotation;
        }

        public bool GetIsTextAnnotation()
        {
            return textAnnotation;
        }

        public void SetIsAreaAnnotation(bool isAreaAnnotation)
        {
            this.areaAnnotation = isAreaAnnotation;
        }

        public bool GetIsAreaAnnotation()
        {
            return areaAnnotation;
        }

        public void SetIsPointAnnotation(bool isPointAnnotation)
        {
            this.pointAnnotation = isPointAnnotation;
        }

        public bool GetIsPointAnnotation()
        {
            return pointAnnotation;
        }

        public void SetIsTextStrikeoutAnnotation(bool isTextStrikeoutAnnotation)
        {
            this.textStrikeoutAnnotation = isTextStrikeoutAnnotation;
        }

        public bool GetIsTextStrikeoutAnnotation()
        {
            return textStrikeoutAnnotation;
        }

        public void SetIsPolylineAnnotation(bool isPolylineAnnotation)
        {
            this.polylineAnnotation = isPolylineAnnotation;
        }

        public bool GetIsPolylineAnnotation()
        {
            return polylineAnnotation;
        }

        public void SetIsTextFieldAnnotation(bool isTextFieldAnnotation)
        {
            this.textFieldAnnotation = isTextFieldAnnotation;
        }

        public bool GetIsTextFieldAnnotation()
        {
            return textFieldAnnotation;
        }

        public void SetIsWatermarkAnnotation(bool isWatermarkAnnotation)
        {
            this.watermarkAnnotation = isWatermarkAnnotation;
        }

        public bool GetIsWatermarkAnnotation()
        {
            return watermarkAnnotation;
        }

        public void SetIsTextReplacementAnnotation(bool isTextReplacementAnnotation)
        {
            this.textReplacementAnnotation = isTextReplacementAnnotation;
        }

        public bool GetIsTextReplacementAnnotation()
        {
            return textReplacementAnnotation;
        }

        public void SetIsArrowAnnotation(bool isArrowAnnotation)
        {
            this.arrowAnnotation = isArrowAnnotation;
        }

        public bool GetIsArrowAnnotation()
        {
            return arrowAnnotation;
        }

        public void SetIsTextRedactionAnnotation(bool isTextRedactionAnnotation)
        {
            this.textRedactionAnnotation = isTextRedactionAnnotation;
        }

        public bool GetIsTextRedactionAnnotation()
        {
            return textRedactionAnnotation;
        }

        public void SetIsResourcesRedactionAnnotation(bool isResourcesRedactionAnnotation)
        {
            this.resourcesRedactionAnnotation = isResourcesRedactionAnnotation;
        }

        public bool GetIsResourcesRedactionAnnotation()
        {
            return resourcesRedactionAnnotation;
        }

        public void SetIsTextUnderlineAnnotation(bool isTextUnderlineAnnotation)
        {
            this.textUnderlineAnnotation = isTextUnderlineAnnotation;
        }

        public bool GetIsTextUnderlineAnnotation()
        {
            return textUnderlineAnnotation;
        }

        public void SetIsDistanceAnnotation(bool isDistanceAnnotation)
        {
            this.distanceAnnotation = isDistanceAnnotation;
        }

        public bool GetIsDistanceAnnotation()
        {
            return distanceAnnotation;
        }

        public void SetIsDownloadOriginal(bool isDownloadOriginal)
        {
            this.downloadOriginal = isDownloadOriginal;
        }

        public bool GetIsDownloadOriginal()
        {
            return downloadOriginal;
        }

        public void SetIsDownloadAnnotated(bool isDownloadAnnotated)
        {
            this.downloadAnnotated = isDownloadAnnotated;
        }

        public bool GetIsDownloadAnnotated()
        {
            return downloadAnnotated;
        }

        public void SetIsZoom(bool isZoom)
        {
            this.zoom = isZoom;
        }

        public bool GetIsZoom()
        {
            return zoom;
        }

        public void SetIsFitWidth(bool isFitWidth)
        {
            this.fitWidth = isFitWidth;
        }

        public bool GetIsFitWidth()
        {
            return fitWidth;
        }
    }
}