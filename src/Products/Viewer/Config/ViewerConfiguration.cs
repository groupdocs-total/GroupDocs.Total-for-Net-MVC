using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Viewer.Config
{
    /// <summary>
    /// ViewerConfiguration
    /// </summary>
    public class ViewerConfiguration
    {
        public string FilesDirectory = "DocumentSamples";
        public string FontsDirectory = "";
        public string DefaultDocument = "";
        public int PreloadPageCount = 0;
        public bool isZoom = true;
        public bool isSearch = true;
        public bool isThumbnails = true;
        public bool isRotate = true;
        public bool isHtmlMode = true;
        public bool Cache = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewerConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("viewer");

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
            FontsDirectory = (configuration != null && !String.IsNullOrEmpty(configuration["fontsDirectory"].ToString())) ?
                configuration["fontsDirectory"] :
                FontsDirectory;
            DefaultDocument = (configuration != null && !String.IsNullOrEmpty(configuration["defaultDocument"].ToString())) ?
                configuration["defaultDocument"] :
                DefaultDocument;
            PreloadPageCount = (configuration != null && !String.IsNullOrEmpty(configuration["preloadPageCount"].ToString())) ?
                Convert.ToInt32(configuration["preloadPageCount"]) : 
                PreloadPageCount;
            isZoom = (configuration != null && !String.IsNullOrEmpty(configuration["zoom"].ToString())) ? 
                Convert.ToBoolean(configuration["zoom"]) :
                isZoom;
            isSearch = (configuration != null && !String.IsNullOrEmpty(configuration["search"].ToString())) ? 
                Convert.ToBoolean(configuration["search"]) : 
                isSearch;
            isThumbnails = (configuration != null && !String.IsNullOrEmpty(configuration["thumbnails"].ToString())) ?
                Convert.ToBoolean(configuration["thumbnails"]) : 
                isThumbnails;
            isRotate = (configuration != null && !String.IsNullOrEmpty(configuration["rotate"].ToString())) ? 
                Convert.ToBoolean(configuration["rotate"]) : 
                isRotate;
            isHtmlMode = (configuration != null && !String.IsNullOrEmpty(configuration["htmlMode"].ToString())) ? 
                Convert.ToBoolean(configuration["htmlMode"]) : 
                isHtmlMode;
            Cache = (configuration != null && !String.IsNullOrEmpty(configuration["cache"].ToString())) ?
                Convert.ToBoolean(configuration["cache"]) : 
                Cache;
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