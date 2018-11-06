using GroupDocs.Metadata.Formats.Video;
using GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class MovMetadataImporter : BaseMetadataImporter
    {
        public MovMetadataImporter(Stream file)
            : base(file)
        {
        }

        public override string ImportMetadata()
        {            
            try
            {
                string jsonString = String.Empty;
                // get Mov info
                using (MovFormat movFormat = new MovFormat(Document))
                {
                    Dictionary<string, object> metadata = new MetadataConverter(movFormat.QuickTimeInfo).ConvertMovValues();
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