using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Comparison.Config
{
    /// <summary>
    /// CommonConfiguration
    /// </summary>
    public class ComparisonConfiguration : ConfigurationSection
    {
        public string FilesDirectory { get; set; }
        public string ResultDirectory { get; set; }
        public int PreloadResultPageCount { get; set; }
        public bool isMultiComparing { get; set; }       
        private NameValueCollection comparisonConfiguration = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("comparisonConfiguration");

        /// <summary>
        /// Constructor
        /// </summary>
        public ComparisonConfiguration()
        {
            // get Comparison configuration section from the web.config           
            FilesDirectory = comparisonConfiguration["filesDirectory"];
            if (!IsFullPath(FilesDirectory))
            {
                FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, comparisonConfiguration["filesDirectory"]);
                if (!Directory.Exists(FilesDirectory))
                {
                    FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DocumentSamples");
                    Directory.CreateDirectory(FilesDirectory);
                }
            }
            ResultDirectory = comparisonConfiguration["resultDirectory"];
            if (!IsFullPath(ResultDirectory))
            {
                ResultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, comparisonConfiguration["resultDirectory"]);
                if (!Directory.Exists(ResultDirectory))
                {
                    ResultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DocumentSamples/compared");
                    Directory.CreateDirectory(ResultDirectory);
                }
            }
            PreloadResultPageCount = Convert.ToInt32(comparisonConfiguration["preloadResultPageCount"]);
            isMultiComparing = Convert.ToBoolean(comparisonConfiguration["isMultiComparing"]);         
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