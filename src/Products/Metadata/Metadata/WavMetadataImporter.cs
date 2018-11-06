using GroupDocs.Metadata.Formats.Audio;
using GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class WavMetadataImporter : BaseMetadataImporter
    {
        public WavMetadataImporter(Stream file)
            : base(file)
        {
        }

        public override string ImportMetadata()
        {            
            try
            {
                string jsonString = String.Empty;
                // get MPEG audio info
                using (WavFormat wavFormat = new WavFormat(Document))
                {
                    Dictionary<string, object> metadata = new MetadataConverter(wavFormat.AudioInfo).ConvertValues();
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