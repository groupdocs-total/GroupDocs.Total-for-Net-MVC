using GroupDocs.Total.MVC.Products.Common.Util.Directory;
using GroupDocs.Total.MVC.Products.Signature.Config;
using System;

namespace GroupDocs.Total.MVC.Products.Signature.Util.Directory
{
    /// <summary>
    /// OutputDirectoryUtils
    /// </summary>
    public class TempDirectoryUtils : IDirectoryUtils
    {
        private String OUTPUT_FOLDER = "/SignedTemp";
        private SignatureConfiguration signatureConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public TempDirectoryUtils(SignatureConfiguration signatureConfiguration)
        {
            this.signatureConfiguration = signatureConfiguration;

            // create output directories
            if (String.IsNullOrEmpty(signatureConfiguration.TempFilesDirectory))
            {
                signatureConfiguration.TempFilesDirectory = signatureConfiguration.FilesDirectory + OUTPUT_FOLDER;
            }
        }

        /// <summary>
        /// Get path
        /// </summary>
        /// <returns>string</returns>
        public string GetPath()
        {
            return signatureConfiguration.TempFilesDirectory;
        }
    }
}