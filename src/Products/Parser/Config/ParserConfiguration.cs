using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Parser.Config
{
    /// <summary>
    /// Parser Configuration
    /// </summary>
    public class ParserConfiguration : CommonConfiguration
    {
        [JsonProperty]
        private readonly string filesDirectory = "DocumentSamples/Parser";
    
        [JsonProperty]
        private readonly int preloadPageCount;

        public ParserConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("parser");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);

            filesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", filesDirectory);
            if (!IsFullPath(filesDirectory))
            {
                filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filesDirectory);
                if (!Directory.Exists(filesDirectory))
                {
                    Directory.CreateDirectory(filesDirectory);
                }
            }
        }

        private static bool IsFullPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }

        public string GetFilesDirectory()
        {
            return filesDirectory;
        }

        public int GetPreloadPageCount()
        {
            return preloadPageCount;
        }
    }
}