using GroupDocs.Total.MVC.Products.Signature.Config;

namespace GroupDocs.Total.MVC.Products.Signature.Util.Directory
{
    /// <summary>
    /// DirectoryUtils
    /// </summary>
    public class DirectoryUtils
    {
        public FilesDirectoryUtils FilesDirectory;
        public DataDirectoryUtils DataDirectory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public DirectoryUtils(SignatureConfiguration signatureConfiguration)
        {
            FilesDirectory = new FilesDirectoryUtils(signatureConfiguration);
            DataDirectory = new DataDirectoryUtils(signatureConfiguration);
        }
    }
}