using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GroupDocs.Total.MVC.Products.Metadata.Context;
using GroupDocs.Total.MVC.Products.Metadata.DTO;
using GroupDocs.Total.MVC.Products.Metadata.Model;
using GroupDocs.Total.MVC.Products.Metadata.Util;

namespace GroupDocs.Total.MVC.Products.Metadata.Services
{
    public class MetadataService
    {
        private readonly HashSet<PropertyType> arrayTypes = new HashSet<PropertyType>
        {
            PropertyType.PropertyValueArray,
            PropertyType.StringArray,
            PropertyType.ByteArray,
            PropertyType.DoubleArray,
            PropertyType.IntegerArray,
            PropertyType.LongArray,
        };

        public IEnumerable<ExtractedPackageDto> GetPackages(PostedDataDto postedData)
        {
            using (MetadataContext context = new MetadataContext(postedData.guid, postedData.password))
            {
                var packages = new List<ExtractedPackageDto>();
                foreach (var package in context.GetPackages())
                {
                    List<PropertyDto> properties = new List<PropertyDto>();
                    List<KnownPropertyDto> descriptors = new List<KnownPropertyDto>();

                    foreach (var property in package.Properties)
                    {
                        properties.Add(new PropertyDto
                        {
                            name = property.Name,
                            value = property.Value is Array ? ArrayUtil.AsString((Array)property.Value) : property.Value,
                            type = (int)property.Type,
                        });
                    }

                    foreach (var descriptor in package.Descriptors)
                    {
                        var accessLevel = descriptor.AccessLevel;
                        if (arrayTypes.Contains(descriptor.Type))
                        {
                            accessLevel &= AccessLevels.Remove;
                        }

                        descriptors.Add(new KnownPropertyDto
                        {
                            name = descriptor.Name,
                            type = (int)descriptor.Type,
                            accessLevel = (int)accessLevel
                        });
                    }

                    packages.Add(new ExtractedPackageDto
                    {
                        id = package.Id,
                        name = package.Name,
                        index = package.Index,
                        type = (int)package.Type,
                        properties = properties,
                        knownProperties = descriptors,
                    });

                }
                return packages;
            }
        }

        public void SaveProperties(PostedDataDto postedData)
        {
            var tempFilePath = GetTempPath(postedData);
            using (MetadataContext context = new MetadataContext(postedData.guid, postedData.password))
            {
                foreach (var packageInfo in postedData.packages)
                {
                    context.UpdateProperties(packageInfo.id, packageInfo.properties.Select(p => new Property(p.name, (PropertyType)p.type, p.value)));
                }
                context.Save(tempFilePath);
            }

            DirectoryUtils.MoveFile(tempFilePath, postedData.guid);
        }

        public void RemoveProperties(PostedDataDto postedData)
        {
            var tempFilePath = GetTempPath(postedData);
            using (MetadataContext context = new MetadataContext(postedData.guid, postedData.password))
            {
                foreach (var packageInfo in postedData.packages)
                {
                    context.RemoveProperties(packageInfo.id, packageInfo.properties.Select(p => p.name));
                }
                context.Save(tempFilePath);
            }
            DirectoryUtils.MoveFile(tempFilePath, postedData.guid);
        }

        private static string GetTempPath(PostedDataDto postedData)
        {
            string tempFilename = Path.GetFileNameWithoutExtension(postedData.guid) + "_tmp";
            return Path.Combine(Path.GetDirectoryName(postedData.guid), tempFilename + Path.GetExtension(postedData.guid));
        }
    }
}