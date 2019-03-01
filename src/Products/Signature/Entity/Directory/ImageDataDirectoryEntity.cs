using GroupDocs.Total.MVC.Products.Signature.Config;

namespace GroupDocs.Total.MVC.Products.Signature.Entity.Directory
{
    /// <summary>
    /// ImageDataDirectoryEntity
    /// </summary>
    public class ImageDataDirectoryEntity : DataDirectoryEntity
    {
        public string UPLOADED_IMAGE = "/Uploaded";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public ImageDataDirectoryEntity(SignatureConfiguration signatureConfiguration)
            : base(signatureConfiguration, "/Image")
        {
        }
    }
}