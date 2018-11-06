using GroupDocs.Metadata;
using GroupDocs.Metadata.Formats.Audio;
using GroupDocs.Metadata.Tools;
using GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Metadata.Metadata
{
    public class Mp3MetadataImporter : BaseMetadataImporter
    {
        public Mp3MetadataImporter(Stream file)
            : base(file)
        {
        }

        public override string ImportMetadata()
        {
            try
            {
                Dictionary<string, object> id3v1 = new Dictionary<string, object>();
                Dictionary<string, object> id3v2 = new Dictionary<string, object>();
                Dictionary<string, object> result = new Dictionary<string, object>();
                // get MPEG audio info
                MpegAudio audioInfo = (MpegAudio)MetadataUtility.ExtractSpecificMetadata(Document, MetadataType.MpegAudio);
                Dictionary<string, object> metadata = new MetadataConverter(audioInfo).ConvertValues();
                using (Mp3Format mp3Format = new Mp3Format(Document))
                {
                   id3v1 = new MetadataConverter(mp3Format.Id3v1Properties).ConvertValues();
                   id3v2 = new MetadataConverter(mp3Format.Id3v2Properties).ConvertId3V2Values();
                }
                result = metadata.Concat(id3v1).Concat(id3v2).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value); 
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                return jsonString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}