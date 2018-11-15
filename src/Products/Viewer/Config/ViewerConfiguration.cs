using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Viewer.Config
{
    /// <summary>
    /// ViewerConfiguration
    /// </summary>
    public class ViewerConfiguration : ConfigurationSection
    {
        public static string DEFAULT_FILES_DIRECTORY = "";
        public string FilesDirectory { get; set; }
        public string FontsDirectory { get; set; }
        public string DefaultDocument { get; set; }
        public int PreloadPageCount { get; set; }
        public bool isZoom { get; set; }      
        public bool isSearch { get; set; }
        public bool isThumbnails { get; set; }
        public bool isRotate { get; set; }      
        public bool isHtmlMode { get; set; }
        public bool Cache { get; set; }
        private NameValueCollection viewerConfiguration = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("viewerConfiguration");

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewerConfiguration()
        {
            // get Viewer configuration section from the web.config
            FilesDirectory = viewerConfiguration["filesDirectory"];
            if (!IsFullPath(FilesDirectory))
            {
                FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, viewerConfiguration["filesDirectory"]);
                if (!Directory.Exists(FilesDirectory))
                {
                    FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DocumentSamples");
                    Directory.CreateDirectory(FilesDirectory);
                }
            }
            FontsDirectory = viewerConfiguration["fontsDirectory"];
            DefaultDocument = viewerConfiguration["defaultDocument"];
            PreloadPageCount = Convert.ToInt32(viewerConfiguration["preloadPageCount"]);
            isZoom = Convert.ToBoolean(viewerConfiguration["isZoom"]);
            isSearch = Convert.ToBoolean(viewerConfiguration["isSearch"]);
            isThumbnails = Convert.ToBoolean(viewerConfiguration["isThumbnails"]);
            isRotate = Convert.ToBoolean(viewerConfiguration["isRotate"]);
            isHtmlMode = Convert.ToBoolean(viewerConfiguration["isHtmlMode"]);
            Cache = Convert.ToBoolean(viewerConfiguration["cache"]);
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