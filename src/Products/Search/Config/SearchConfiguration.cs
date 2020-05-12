using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using GroupDocs.Total.MVC.Products.Search.Util.Directory;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Search.Config
{
    /// <summary>
    /// SearchConfiguration
    /// </summary>
    public class SearchConfiguration : CommonConfiguration
    {
        [JsonProperty]
        private string filesDirectory = "DocumentSamples/Search";

        [JsonProperty]
        private string defaultDocument = "";

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchConfiguration()
        {
            YamlParser parser = new YamlParser();
            // get Search configuration section from the web.config
            dynamic configuration = parser.GetConfiguration("search");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);

            filesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", filesDirectory);
            if (!DirectoryUtils.IsFullPath(filesDirectory))
            {
                filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filesDirectory);
                if (!Directory.Exists(filesDirectory))
                {
                    Directory.CreateDirectory(filesDirectory);
                }
            }

            defaultDocument = valuesGetter.GetStringPropertyValue("defaultDocument", defaultDocument);
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
    }
}