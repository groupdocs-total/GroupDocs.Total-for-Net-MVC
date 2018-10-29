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
        /// <param name="document">Stream</param>
        /// <param name="docType">string</param>
        /// <returns></returns>
        public static BaseMetadataImporter CreateMetadataImporter(Stream document, DocumentType docType)
        {
            switch (docType)
            {
                case DocumentType.Doc:
                    return new DocMetadataImporter(document);
                case DocumentType.Xls:
                    return new ExcelMetadataImporter(document);
                case DocumentType.Ppt:
                    return new PowerPointMetadataImporter(document);
                case DocumentType.Pdf:
                    return new PdfMetadataImporter(document);
                case DocumentType.Mp3:
                    return new Mp3MetadataImporter(document);
                case DocumentType.Wav:
                    return new WavMetadataImporter(document);
                case DocumentType.AVI:
                    return new AviMetadataImporter(document);
                case DocumentType.Mov:
                    return new MovMetadataImporter(document);
                default:
                    throw new Exception("Wrong document type!");
            }
        }
    }
}