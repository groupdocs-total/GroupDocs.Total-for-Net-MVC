using GroupDocs.Metadata.Formats.Document;
using GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class DocMetadataImporter : BaseMetadataImporter
    {
        public DocMetadataImporter(Stream document)
            : base(document)
        {
        }

        public override string ImportMetadata()
        {
            DocMetadata docMetadata;
            try
            {
                using (DocFormat docFormat = new DocFormat(Document))
                {
                    // initialize metadata
                    docMetadata = docFormat.DocumentProperties;
                }
                Dictionary<string, object> metadata = new MetadataConverter(docMetadata).ConvertValues();
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