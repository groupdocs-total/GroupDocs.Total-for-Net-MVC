using GroupDocs.Metadata.Formats.Video;
using GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class AviMetadataImporter : BaseMetadataImporter
    {
        public AviMetadataImporter(Stream file)
            : base(file)
        {
        }

        public override string ImportMetadata()
        {            
            try
            {
                string jsonString = String.Empty;
                // get avi info
                using (AviFormat aviFormat = new AviFormat(Document))
                {
                    Dictionary<string, object> metadata = new MetadataConverter(aviFormat.Header).ConvertAviValues();
                    jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(metadata);
                }
                              
                return jsonString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}