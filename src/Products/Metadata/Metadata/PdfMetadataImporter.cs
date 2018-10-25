using GroupDocs.Metadata.Formats.Document;
using GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class PdfMetadataImporter : BaseMetadataImporter
    {
        public PdfMetadataImporter(Stream document)
            : base(document)
        {
        }

        public override string ImportMetadata()
        {
            PdfMetadata pdfMetadata;
            try
            {
                using (PdfFormat pdfFormat = new PdfFormat(Document))
                {
                    // initialize metadata
                    pdfMetadata = pdfFormat.DocumentProperties;
                }
                Dictionary<string, object> metadata = new MetadataConverter(pdfMetadata).ConvertValues();
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(metadata);                
               
                return jsonString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}