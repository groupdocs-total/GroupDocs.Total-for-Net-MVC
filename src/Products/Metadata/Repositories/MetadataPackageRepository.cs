using GroupDocs.Metadata.Common;
using System;
using System.Collections.Generic;


namespace GroupDocs.Total.MVC.Products.Metadata.Repositories
{
    public abstract class MetadataPackageRepository
    {
        private readonly HashSet<MetadataPropertyType> supportedPropertyTypes = new HashSet<MetadataPropertyType>
        {
            MetadataPropertyType.String,
            MetadataPropertyType.DateTime,
            MetadataPropertyType.Integer,
            MetadataPropertyType.Long,
            MetadataPropertyType.Double,
            MetadataPropertyType.Boolean,

            MetadataPropertyType.PropertyValueArray,
            MetadataPropertyType.StringArray,
            MetadataPropertyType.ByteArray,
            MetadataPropertyType.DoubleArray,
            MetadataPropertyType.IntegerArray,
            MetadataPropertyType.LongArray
        };

        public MetadataPackageRepository(MetadataPackage package)
        {
            Package = package;
        }

        protected MetadataPackage Package { get; private set; }

        public virtual IEnumerable<PropertyDescriptor> GetDescriptors()
        {
            foreach (var package in GetNestedPackages())
            {
                foreach (var descriptor in package.KnowPropertyDescriptors)
                {
                    if (IsPropertyTypeSupported(descriptor.Type))
                    {
                        yield return descriptor;
                    }
                }
            }
        }

        public virtual IEnumerable<MetadataProperty> GetProperties()
        {
            foreach (var package in GetNestedPackages())
            {
                foreach (var property in package)
                {
                    if (IsPropertyTypeSupported(property.Value.Type))
                    {
                        yield return property;
                    }
                }
            }
        }

        public virtual void RemoveProperty(string name)
        {
            foreach (var package in GetNestedPackages())
            {
                package.RemoveProperties(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            }
        }

        public virtual void SaveProperty(string name, MetadataPropertyType type, dynamic value)
        {
            if (value != null)
            {
                var propertyValue = CreatePropertyValue(type, value);
                GroupDocs.Metadata.Common.Func<MetadataProperty, bool> condition = p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase);
                foreach (var package in GetNestedPackages())
                {
                    package.SetProperties(condition, propertyValue);
                }
            }
        }

        protected abstract IEnumerable<MetadataPackage> GetNestedPackages();

        protected bool IsPropertyTypeSupported(MetadataPropertyType type)
        {
            return supportedPropertyTypes.Contains(type);
        }

        protected PropertyValue CreatePropertyValue(MetadataPropertyType type, dynamic value)
        {
            switch (type)
            {
                case MetadataPropertyType.Integer:
                    if (value is long)
                    {
                        return new PropertyValue((int)value);
                    }
                    break;
                case MetadataPropertyType.Double:
                    if (value is long)
                    {
                        return new PropertyValue((double)value);
                    }
                    break;
            }
            return new PropertyValue(value);
        }
    }
}