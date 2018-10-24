using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public abstract class BaseMetadataImporter
    {
        protected Stream Document;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="document">Stream</param>       
        public BaseMetadataImporter(Stream document)
        {
            Document = document;          
        }

        /// <summary>
        /// Import Word document metadata
        /// </summary> 
        /// <returns>DocMetadata</returns>
        public abstract string ImportMetadata();          
    }
}