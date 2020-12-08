using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using GroupDocs.Total.MVC.Products.Metadata.Util;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Config
{
    /// <summary>
    /// MetadataConfiguration
    /// </summary>
    public class MetadataConfiguration : CommonConfiguration
    {
        private string filesDirectory = "DocumentSamples/Metadata";

        private string outputDirectory = "DocumentSamples/Metadata/Output";

        private int previewTimeLimit;

        [JsonProperty]
        private string defaultDocument = "";

        [JsonProperty]
        private int preloadPageCount;

        [JsonProperty]
        private bool htmlMode = true;

        [JsonProperty]
        private bool cache = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("metadata");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);

            // get Metadata configuration section from the web.config
            filesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", filesDirectory);
            filesDirectory = InitDirectory(filesDirectory);
            
            outputDirectory = valuesGetter.GetStringPropertyValue("outputDirectory", outputDirectory);
            outputDirectory = InitDirectory(outputDirectory);

            defaultDocument = valuesGetter.GetStringPropertyValue("defaultDocument", defaultDocument);
            preloadPageCount = valuesGetter.GetIntegerPropertyValue("preloadPageCount", preloadPageCount);
            previewTimeLimit = valuesGetter.GetIntegerPropertyValue("previewTimeLimit", previewTimeLimit);
            htmlMode = valuesGetter.GetBooleanPropertyValue("htmlMode", htmlMode);
            cache = valuesGetter.GetBooleanPropertyValue("cache", cache);
            browse = valuesGetter.GetBooleanPropertyValue("browse", browse);
            upload = valuesGetter.GetBooleanPropertyValue("upload", upload);
        }

        public void SetFilesDirectory(string filesDirectory)
        {
            this.filesDirectory = filesDirectory;
        }

        public string GetFilesDirectory()
        {
            return filesDirectory;
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

        public int GetPreviewTimeLimit()
        {
            return previewTimeLimit;
        }

        public void SetIsHtmlMode(bool isHtmlMode)
        {
            htmlMode = isHtmlMode;
        }

        public bool GetIsHtmlMode()
        {
            return htmlMode;
        }

        public void SetCache(bool Cache)
        {
            this.cache = Cache;
        }

        public bool GetCache()
        {
            return cache;
        }

        public string GetInputFilePath(string relativePath)
        {
            return GetAbsolutePath(filesDirectory, relativePath);
        }

        public string GetOutputFilePath(string relativePath)
        {
            return GetAbsolutePath(outputDirectory, relativePath);
        }

        private string GetAbsolutePath(string baseDirectory, string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException("Couldn't find the specified file", nameof(relativePath));
            }
            return Path.Combine(baseDirectory, relativePath);
        }

        private string InitDirectory(string path)
        {
            string absolutePath = path;
            if (!Path.IsPathRooted(path))
            {
                absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
            }

            return absolutePath;
        }
    }
}