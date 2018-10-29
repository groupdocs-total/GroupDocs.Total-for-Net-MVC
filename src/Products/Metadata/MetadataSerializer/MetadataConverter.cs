using GroupDocs.Metadata;
using GroupDocs.Metadata.Formats.Audio;
using System;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer
{
    public class MetadataConverter
    {
        public Dictionary<string, object> MetadataProperties;
        private dynamic Metadata;

        public MetadataConverter(dynamic metadata)
        {
            Metadata = metadata;
        }

        public Dictionary<string, object> ConvertValues()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (var property in Metadata)
            {
                PropertyValue value = property.Value;
                if (value != null)
                {
                    string propertyName = String.Empty;
                    try
                    {
                        propertyName = property.Key;
                    }
                    catch (Exception)
                    {
                        propertyName = property.Name;
                    }
                    switch (value.Type.ToString())
                    {
                        case "String":
                            if (!String.IsNullOrEmpty(value.ToString()))
                            {
                                values[propertyName] = value.ToString();
                            }
                            break;
                        case "Integer":
                            values[propertyName] = value.ToInt();
                            break;
                        case "Boolean":
                            values[propertyName] = value.ToBool();
                            break;
                        case "ByteArray":
                            values[propertyName] = value.ToByteArray();
                            break;
                        case "DateTime":
                            values[propertyName] = value.ToDateTime();
                            break;
                        case "Double":
                            values[propertyName] = value.ToDouble();
                            break;
                        case "Long":
                            values[propertyName] = value.ToLong();
                            break;
                        case "StringArray":
                            values[propertyName] = value.ToStringArray();
                            break;
                        case "TimeSpan":
                            values[propertyName] = value.ToTimeSpan();
                            break;
                        default:
                            values[propertyName] = value.ToString();
                            break;
                    }
                }
            }
            return values;
        }

        internal Dictionary<string, object> ConvertMovValues()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            for(int i = 0; i < Metadata.Atoms.Length; i++)
            {
                object atom = new
                {
                    Name = Metadata.Atoms[i].Name,
                    Offset = Metadata.Atoms[i].Offset,
                    Data = Metadata.Atoms[i].Data,
                    Size = Metadata.Atoms[i].Size,
                    Type = Metadata.Atoms[i].Type
                };
                // display video width
                values["Atom" + i] = atom;               
            }
            return values;
        }

        internal Dictionary<string, object> ConvertAviValues()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            // display video width
            values["Width"] =  Metadata.Width;

            // display video height
            values["Height"] = Metadata.Height;

            // display total frames
            values["Total frames"] = Metadata.TotalFrames;

            // display number of streams in file
            values["Number of streams"] = Metadata.Streams;

            // display suggested buffer size for reading the file
            values["Suggested buffer size"] = Metadata.SuggestedBufferSize;
            return values;
        }

        public Dictionary<string, object> ConvertId3V2Values()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (TagFrame tagFrame in Metadata)
            {
                if (tagFrame != null)
                {
                    values[tagFrame.Name] = tagFrame.GetFormattedValue();
                }
            }
            return values;
        }
    }
}