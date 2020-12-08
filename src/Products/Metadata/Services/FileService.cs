using GroupDocs.Total.MVC.Products.Metadata.Config;
using GroupDocs.Total.MVC.Products.Metadata.Context;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Services
{
    public class FileService
    {
        private readonly MetadataConfiguration metadataConfiguration;

        public FileService(MetadataConfiguration metadataConfiguration)
        {
            this.metadataConfiguration = metadataConfiguration;
        }

        public Stream GetSourceFileStream(string relativePath)
        {
            string filePath = metadataConfiguration.GetInputFilePath(relativePath);
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Stream GetFileInputStream(string relativePath)
        {
            var outputPath = metadataConfiguration.GetOutputFilePath(relativePath);
            if (File.Exists(outputPath))
            {
                return File.Open(outputPath, FileMode.Open, FileAccess.ReadWrite);
            }
            var inputPath = metadataConfiguration.GetInputFilePath(relativePath);
            return File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public void SaveContextToFile(MetadataContext context, string relativePath)
        {
            var outputPath = metadataConfiguration.GetOutputFilePath(relativePath);
            if (File.Exists(outputPath))
            {
                context.Save();
            }
            else
            {
                context.Save(outputPath);
            }
        }
    }
}