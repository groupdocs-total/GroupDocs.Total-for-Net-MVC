using System;
using System.Linq;
using System.Collections.Generic;
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Options;
using GroupDocs.Total.MVC.Products.Metadata.Model;
using GroupDocs.Total.MVC.Products.Metadata.Repositories;

namespace GroupDocs.Total.MVC.Products.Metadata.Context
{
    public class MetadataContext : IDisposable
    {
        private readonly GroupDocs.Metadata.Metadata metadata;

        private bool disposedValue;

        public MetadataContext(string filePath, string password)
        {
            var loadOptions = new LoadOptions
            {
                Password = password == string.Empty ? null : password
            };

            metadata = new GroupDocs.Metadata.Metadata(filePath, loadOptions);
        }

        public IEnumerable<Package> GetPackages()
        {
            var root = metadata.GetRootPackage();
            var packages = new List<Package>();
            foreach (var rootProperty in root)
            {
                if (rootProperty.Value != null && rootProperty.Value.Type == MetadataPropertyType.Metadata)
                {
                    var package = rootProperty.Value.ToClass<MetadataPackage>();

                    var repository = MetadataRepositoryFactory.Create(package);

                    var properties = new List<Property>();
                    var descriptors = new List<Model.PropertyDescriptor>();

                    foreach (var property in repository.GetProperties().OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        var value = property.Value;
                        properties.Add(property);
                    }

                    foreach (var descriptor in repository.GetDescriptors().OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        var accessLevel = descriptor.AccessLevel;

                        descriptors.Add(descriptor);
                    }

                    if (properties.Count > 0 || descriptors.Any(descriptor => (descriptor.AccessLevel & AccessLevels.Add) != 0))
                    {
                        packages.Add(new Package(rootProperty.Name, (PackageType)package.MetadataType, properties, descriptors));
                    }

                }
            }

            return packages;
        }

        public void UpdateProperties(string packageName, IEnumerable<Property> properties)
        {
            var root = metadata.GetRootPackage();

            var package = root[packageName].Value.ToClass<MetadataPackage>();
            var repository = MetadataRepositoryFactory.Create(package);
            foreach (var property in properties)
            {
                repository.SaveProperty(property.Name, property.Type, property.Value);
            }
        }

        public void RemoveProperties(string packageName, IEnumerable<string> properties)
        {
            var root = metadata.GetRootPackage();

            var repository = MetadataRepositoryFactory.Create(root[packageName].Value.ToClass<MetadataPackage>());
            foreach (var propertyName in properties)
            {
                repository.RemoveProperty(propertyName);
            }
        }

        public void Save(string filePath)
        {
            metadata.Save(filePath);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    metadata.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}