using GroupDocs.Metadata;
using System;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class MetadataFactory
    {
        /// <summary>
        /// Create MetadataImporter instance depending on type of the document
        /// </summary>
        /// <param name="documentPath">string</param>
        /// <param name="docType">string</param>
        /// <returns></returns>
        public static BaseMetadataImporter CreateMetadataImporter(Stream document, DocumentType docType)
        {
            switch (docType)
            {
                case DocumentType.Doc:
                    return new DocMetadataImporter(document);
                default:
                    throw new Exception("Wrong document type!");
            }
        }
    }
}