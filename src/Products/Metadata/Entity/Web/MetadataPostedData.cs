using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.Entity.Web
{
    public class MetadataPostedData : PostedDataEntity
    {
        /// <summary>
        /// Collection of the document properties with their data.
        /// </summary>
        public List<PostedMetadataPackage> packages { get; set; }
    }
}