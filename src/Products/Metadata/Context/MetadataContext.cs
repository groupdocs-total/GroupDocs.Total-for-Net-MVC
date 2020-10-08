﻿using System;
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

        private readonly MetadataPathConfig metadataPathConfig;

        private bool disposedValue;

        public MetadataContext(string filePath, string password)
        {
            var loadOptions = new LoadOptions
            {
                Password = password == string.Empty ? null : password
            };

            metadata = new GroupDocs.Metadata.Metadata(filePath, loadOptions);
            metadataPathConfig = new MetadataPathConfig();
        }

        public IEnumerable<Package> GetPackages()
        {
            var root = metadata.GetRootPackage();
            var packages = new List<Package>();
            foreach (var branchProperty in root)
            {
                if (branchProperty.Value != null && branchProperty.Value.Type == MetadataPropertyType.Metadata)
                {
                    var branchPackage = branchProperty.Value.ToClass<MetadataPackage>();

                    foreach (var nestedPackageInfo in metadataPathConfig.GetRegisteredPackages(branchPackage))
                    {
                        var repository = MetadataRepositoryFactory.Create(nestedPackageInfo.Package);
                        var properties = new List<Property>();
                        var descriptors = new List<Model.PropertyDescriptor>();

                        foreach (var property in repository.GetProperties().OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            properties.Add(property);
                        }

                        foreach (var descriptor in repository.GetDescriptors().OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            descriptors.Add(descriptor);
                        }

                        if (properties.Count > 0 || descriptors.Any(descriptor => (descriptor.AccessLevel & AccessLevels.Add) != 0))
                        {
                            packages.Add(
                                new Package(metadataPathConfig.CombinePath(branchProperty.Name, nestedPackageInfo.Path),
                                nestedPackageInfo.Package.GetType().Name,
                                nestedPackageInfo.Index,
                                (PackageType)nestedPackageInfo.Package.MetadataType,
                                properties,
                                descriptors));
                        }
                    }
                }
            }

            return packages;
        }

        public void UpdateProperties(string packageName, IEnumerable<Property> properties)
        {
            var root = metadata.GetRootPackage();

            var package = metadataPathConfig.GetPackageByPath(root, packageName);
            var repository = MetadataRepositoryFactory.Create(package);
            foreach (var property in properties)
            {
                repository.SaveProperty(property.Name, property.Type, property.Value);
            }
        }

        public void RemoveProperties(string packageName, IEnumerable<string> properties)
        {
            var root = metadata.GetRootPackage();

            var package = metadataPathConfig.GetPackageByPath(root, packageName);
            var repository = MetadataRepositoryFactory.Create(package);
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