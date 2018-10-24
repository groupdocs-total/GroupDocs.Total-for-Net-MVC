using GroupDocs.Metadata;
using System;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.MetadataSerializer
{
    public class MetadataConverter
    {
        public Dictionary<string, object> MetadataProperties;
        private dynamic Metadata;

        public MetadataConverter(dynamic metadata) {
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
                    switch (value.Type.ToString())
                    {
                        case "String":
                            values[property.Key] = value.ToString();
                            break;
                        case "Integer":
                            values[property.Key] = value.ToInt();
                            break;
                        case "Boolean":
                            values[property.Key] = value.ToBool();
                            break;
                        case "ByteArray":
                            values[property.Key] = value.ToByteArray();
                            break;
                        case "DateTime":
                            values[property.Key] = value.ToDateTime();
                            break;
                        case "Double":
                            values[property.Key] = value.ToDouble();
                            break;
                        case "Long":
                            values[property.Key] = value.ToLong();
                            break;
                        case "StringArray":
                            values[property.Key] = value.ToStringArray();
                            break;
                        case "TimeSpan":
                            values[property.Key] = value.ToTimeSpan();
                            break;
                        default:
                            values[property.Key] = value.ToString();
                            break;
                    }
                }
            }
            return values;
        }
    }
}