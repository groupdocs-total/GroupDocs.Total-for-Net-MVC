using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Signature.Config
{
    /// <summary>
    /// SignatureConfiguration
    /// </summary>
    public class SignatureConfiguration
    {
        public string FilesDirectory = "DocumentSamples";
        public string OutputDirectory = "";
        public string DefaultDocument = "";
        public string DataDirectory = "";
        public int PreloadPageCount = 0;
        public bool isTextSignature = true;
        public bool isImageSignature = true;
        public bool isDigitalSignature = true;
        public bool isQrCodeSignature = true;
        public bool isBarCodeSignature = true;
        public bool isStampSignature = true;
        public bool isDownloadOriginal = true;
        public bool isDownloadSigned = true;      

        /// <summary>
        /// Get signature configuration section from the Web.config
        /// </summary>
        public SignatureConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("signature");
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
            OutputDirectory = (configuration != null && !String.IsNullOrEmpty(configuration["outputDirectory"].ToString())) ? configuration["outputDirectory"] : OutputDirectory;
            DataDirectory = (configuration != null && !String.IsNullOrEmpty(configuration["dataDirectory"].ToString())) ? configuration["dataDirectory"] : DataDirectory;
            isTextSignature = (configuration != null && !String.IsNullOrEmpty(configuration["textSignature"].ToString())) ? Convert.ToBoolean(configuration["textSignature"]) : isTextSignature;
            isImageSignature = (configuration != null && !String.IsNullOrEmpty(configuration["imageSignature"].ToString())) ? Convert.ToBoolean(configuration["imageSignature"]) : isImageSignature;
            isDigitalSignature = (configuration != null && !String.IsNullOrEmpty(configuration["digitalSignature"].ToString())) ? Convert.ToBoolean(configuration["digitalSignature"]) : isDigitalSignature;
            isQrCodeSignature = (configuration != null && !String.IsNullOrEmpty(configuration["qrCodeSignature"].ToString())) ? Convert.ToBoolean(configuration["qrCodeSignature"]) : isQrCodeSignature;
            isBarCodeSignature = (configuration != null && !String.IsNullOrEmpty(configuration["barCodeSignature"].ToString())) ? Convert.ToBoolean(configuration["barCodeSignature"]) : isBarCodeSignature;
            isStampSignature = (configuration != null && !String.IsNullOrEmpty(configuration["stampSignature"].ToString())) ? Convert.ToBoolean(configuration["stampSignature"]) : isStampSignature;
            isDownloadOriginal = (configuration != null && !String.IsNullOrEmpty(configuration["downloadOriginal"].ToString())) ? Convert.ToBoolean(configuration["downloadOriginal"]) : isDownloadOriginal;
            isDownloadSigned = (configuration != null && !String.IsNullOrEmpty(configuration["downloadSigned"].ToString())) ? Convert.ToBoolean(configuration["downloadSigned"]) : isDownloadSigned;
            DefaultDocument = (configuration != null && !String.IsNullOrEmpty(configuration["defaultDocument"].ToString())) ? configuration["defaultDocument"] : DefaultDocument;
            PreloadPageCount = (configuration != null && !String.IsNullOrEmpty(configuration["preloadPageCount"].ToString())) ? Convert.ToInt32(configuration["preloadPageCount"]) : PreloadPageCount;
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