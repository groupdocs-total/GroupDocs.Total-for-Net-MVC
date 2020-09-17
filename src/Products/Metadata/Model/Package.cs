

using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.Model
{
    public class Package
    {
        public Package(string name, PackageType type, IEnumerable<Property> properties, IEnumerable<PropertyDescriptor> descriptors)
        {
            Name = name;
            Type = type;
            Properties = properties;
            Descriptors = descriptors;
        }

        public string Name { get; private set; }

        public PackageType Type { get; private set; }

        public IEnumerable<Property> Properties { get; private set; }

        public IEnumerable<PropertyDescriptor> Descriptors { get; private set; }
    }
}