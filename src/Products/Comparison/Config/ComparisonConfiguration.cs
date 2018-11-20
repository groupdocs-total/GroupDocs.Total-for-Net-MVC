using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Comparison.Config
{
    /// <summary>
    /// CommonConfiguration
    /// </summary>
    public class ComparisonConfiguration
    {
        public string FilesDirectory = "DocumentSamples";
        public string ResultDirectory = "Compared";
        public int PreloadResultPageCount = 0;
        public bool isMultiComparing = true;      

        /// <summary>
        /// Constructor
        /// </summary>
        public ComparisonConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("comparison");
            // get Comparison configuration section from the web.config            
            FilesDirectory = (configuration != null && !String.IsNullOrEmpty(configuration["filesDirectory"].ToString())) ? configuration["filesDirectory"] : FilesDirectory;
            if (!IsFullPath(FilesDirectory))
            {
                FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilesDirectory);
                if (!Directory.Exists(FilesDirectory))
                {
                    Directory.CreateDirectory(FilesDirectory);
                }
            }
            ResultDirectory = (configuration != null && !String.IsNullOrEmpty(configuration["resultDirectory"].ToString())) ? configuration["resultDirectory"] : ResultDirectory;
            if (!IsFullPath(ResultDirectory))
            {
                ResultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResultDirectory);
                if (!Directory.Exists(ResultDirectory))
                {                    
                    Directory.CreateDirectory(ResultDirectory);
                }
            }
            PreloadResultPageCount = (configuration != null && !String.IsNullOrEmpty(configuration["preloadResultPageCount"].ToString())) ?
                Convert.ToInt32(configuration["preloadResultPageCount"]) : 
                PreloadResultPageCount;
            isMultiComparing = (configuration != null && !String.IsNullOrEmpty(configuration["multiComparing"].ToString())) ?
                Convert.ToBoolean(configuration["multiComparing"]) :
                isMultiComparing;
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