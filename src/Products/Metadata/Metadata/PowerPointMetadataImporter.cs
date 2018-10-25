using GroupDocs.Metadata.Formats.Document;
using GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class PowerPointMetadataImporter : BaseMetadataImporter
    {
        public PowerPointMetadataImporter(Stream document)
            : base(document)
        {
        }

        public override string ImportMetadata()
        {
            PptMetadata pptMetadata;
            try
            {
                using (PptFormat pptFormat = new PptFormat(Document))
                {
                    // initialize metadata
                    pptMetadata = pptFormat.DocumentProperties;
                }
                Dictionary<string, object> metadata = new MetadataConverter(pptMetadata).ConvertValues();
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