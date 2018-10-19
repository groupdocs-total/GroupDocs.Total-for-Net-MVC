using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Total.MVC.Products.Metadata.Entity.Web;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Loader
{
    /// <summary>
    /// FilesTreeLoader
    /// </summary>
    public class FilesTreeLoader
    {

        /// <summary>
        /// Load image signatures
        /// </summary>
        /// <returns>List[SignatureFileDescriptionEntity]</returns>
        public List<MetadataFileDescriptionEntity> LoadFiles(string path)
        {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            List<string> allFiles = new List<string>(files);
            List<MetadataFileDescriptionEntity> fileList = new List<MetadataFileDescriptionEntity>();
            try
            {
                allFiles.Sort(new FileTypeComparator());
                allFiles.Sort(new FileNameComparator());

                foreach (string file in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // check if current file/folder is hidden
                    if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        // ignore current file and skip to next one
                        continue;
                    }
                    else
                    {
                        MetadataFileDescriptionEntity fileDescription = new MetadataFileDescriptionEntity();
                        fileDescription.guid = Path.GetFullPath(file);
                        fileDescription.name = Path.GetFileName(file);
                        // set is directory true/false
                        fileDescription.isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory);
                        // set file size
                        fileDescription.size = fileInfo.Length;                      
                        // add object to array list
                        fileList.Add(fileDescription);
                    }
                }
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}