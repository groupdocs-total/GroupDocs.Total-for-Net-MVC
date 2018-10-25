using GroupDocs.Metadata.Formats.Document;
using GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class ExcelMetadataImporter : BaseMetadataImporter
    {
        public ExcelMetadataImporter(Stream document)
            : base(document)
        {
        }

        public override string ImportMetadata()
        {
            XlsMetadata xlsMetadata;
            try
            {
                using (XlsFormat xlsFormat = new XlsFormat(Document))
                {
                    // initialize metadata
                    xlsMetadata = xlsFormat.DocumentProperties;
                }
                Dictionary<string, object> metadata = new MetadataConverter(xlsMetadata).ConvertValues();
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