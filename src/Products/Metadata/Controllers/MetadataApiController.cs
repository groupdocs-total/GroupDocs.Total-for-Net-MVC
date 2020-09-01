using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Exceptions;
using GroupDocs.Metadata.Options;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Total.MVC.Products.Metadata.Config;
using GroupDocs.Total.MVC.Products.Metadata.Entity.Web;
using GroupDocs.Total.MVC.Products.Metadata.Repositories;
using GroupDocs.Total.MVC.Products.Metadata.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GroupDocs.Total.MVC.Products.Metadata.Controllers
{
    /// <summary>
    /// MetadataApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MetadataApiController : ApiController
    {
        private readonly Common.Config.GlobalConfiguration globalConfiguration;

        private readonly HashSet<MetadataPropertyType> arrayTypes = new HashSet<MetadataPropertyType>
        {
            MetadataPropertyType.PropertyValueArray,
            MetadataPropertyType.StringArray,
            MetadataPropertyType.ByteArray,
            MetadataPropertyType.DoubleArray,
            MetadataPropertyType.IntegerArray,
            MetadataPropertyType.LongArray,
        };

        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataApiController()
        {
            // Check if filesDirectory is relative or absolute path
            globalConfiguration = new Common.Config.GlobalConfiguration();
        }

        /// <summary>
        /// Load Metadata configuration
        /// </summary>
        /// <returns>Metadata configuration</returns>
        [HttpGet]
        [Route("metadata/loadConfig")]
        public MetadataConfiguration LoadConfig()
        {
            return globalConfiguration.GetMetadataConfiguration();
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("metadata/loadFileTree")]
        public HttpResponseMessage loadFileTree()
        {
            try
            {
                List<FileDescriptionEntity> filesList = new List<FileDescriptionEntity>();
                if (!string.IsNullOrEmpty(globalConfiguration.GetMetadataConfiguration().GetFilesDirectory()))
                {
                    filesList = LoadFiles(globalConfiguration);
                }
                return Request.CreateResponse(HttpStatusCode.OK, filesList);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Get file properties
        /// </summary>
        /// <param name="postedData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("metadata/loadProperties")]
        public HttpResponseMessage loadProperties(MetadataPostedData postedData)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, GetFileProperties(postedData));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        private IList<ExtractedMetadataPackage> GetFileProperties(MetadataPostedData postedData)
        {
            List<FilePropertyEntity> outputProperties = new List<FilePropertyEntity>();
            string password = string.IsNullOrEmpty(postedData.password) ? null : postedData.password;
            string filePath = postedData.guid;

            // set password for protected document
            var loadOptions = new LoadOptions
            {
                Password = password
            };

            using (GroupDocs.Metadata.Metadata metadata = new GroupDocs.Metadata.Metadata(filePath, loadOptions))
            {
                var root = metadata.GetRootPackage();
                var packages = new List<ExtractedMetadataPackage>();
                foreach (var rootProperty in root)
                {
                    if (rootProperty.Value != null && rootProperty.Value.Type == MetadataPropertyType.Metadata)
                    {
                        var package = rootProperty.Value.ToClass<MetadataPackage>();

                        var repository = MetadataRepositoryFactory.Create(package);

                        List<FilePropertyEntity> properties = new List<FilePropertyEntity>();
                        List<FilePropertyName> knownProperties = new List<FilePropertyName>();

                        foreach (var property in repository.GetProperties().OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            var value = property.Value;
                            properties.Add(new FilePropertyEntity
                            {
                                name = property.Name,
                                value = value.RawValue is Array ? ArrayUtil.AsString((Array)value.RawValue) : value.RawValue,
                                type = value.Type,
                            });
                        }

                        foreach (var knownProperty in repository.GetDescriptors().OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            var accessLevel = knownProperty.AccessLevel;
                            if (arrayTypes.Contains(knownProperty.Type))
                            {
                                accessLevel &= PropertyAccessLevels.Remove;
                            }

                            knownProperties.Add(new FilePropertyName
                            {
                                name = knownProperty.Name,
                                type = knownProperty.Type,
                                accessLevel = accessLevel
                            });
                        }

                        if (properties.Count > 0 || knownProperties.Any(kp => (kp.accessLevel & PropertyAccessLevels.Add) != 0))
                        {
                            packages.Add(new ExtractedMetadataPackage
                            {
                                id = rootProperty.Name,
                                name = package.MetadataType.ToString(),
                                type = package.MetadataType,
                                properties = properties,
                                knownProperties = knownProperties,
                            });
                        }

                    }
                }
                return packages;
            }
        }

        /// <summary>
        /// Save file properties
        /// </summary>
        /// <param name="postedData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("metadata/saveProperty")]
        public HttpResponseMessage saveProperty(MetadataPostedData postedData)
        {
            try
            {
                using (GroupDocs.Metadata.Metadata metadata = new GroupDocs.Metadata.Metadata(postedData.guid, GetLoadOptions(postedData)))
                {
                    if (metadata.FileFormat != FileFormat.Unknown)
                    {
                        var root = metadata.GetRootPackage();

                        foreach (var packageInfo in postedData.packages)
                        {
                            var package = root[packageInfo.id].Value.ToClass<MetadataPackage>();
                            var repository = MetadataRepositoryFactory.Create(package);
                            foreach (var propertyInfo in packageInfo.properties)
                            {
                                repository.SaveProperty(propertyInfo.name, propertyInfo.type, propertyInfo.value);
                            }
                        }

                        metadata.Save(GetTempPath(postedData));
                    }
                }

                if (File.Exists(postedData.guid))
                {
                    File.Delete(postedData.guid);
                }

                File.Move(GetTempPath(postedData), postedData.guid);

                // TODO: consider option to response with updated file
                return Request.CreateResponse(HttpStatusCode.OK, new object());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Remove file properties
        /// </summary>
        /// <param name="postedData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("metadata/removeProperty")]
        public HttpResponseMessage removeProperty(MetadataPostedData postedData)
        {
            try
            {
                using (GroupDocs.Metadata.Metadata metadata = new GroupDocs.Metadata.Metadata(postedData.guid, GetLoadOptions(postedData)))
                {
                    if (metadata.FileFormat != FileFormat.Unknown)
                    {
                        var root = metadata.GetRootPackage();
                        var postedPackage = postedData.packages[0];
                        var repository = MetadataRepositoryFactory.Create(root[postedPackage.id].Value.ToClass<MetadataPackage>());
                        repository.RemoveProperty(postedPackage.properties[0].name);

                        metadata.Save(GetTempPath(postedData));
                    }
                }

                if (File.Exists(postedData.guid))
                {
                    File.Delete(postedData.guid);
                }

                File.Move(GetTempPath(postedData), postedData.guid);

                return Request.CreateResponse(HttpStatusCode.OK, new object());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load documents
        /// </summary>
        /// <returns>List[FileDescriptionEntity]</returns>
        public static List<FileDescriptionEntity> LoadFiles(Common.Config.GlobalConfiguration globalConfiguration)
        {
            var currentPath = globalConfiguration.GetMetadataConfiguration().GetFilesDirectory();
            List<string> allFiles = new List<string>(Directory.GetFiles(currentPath));
            allFiles.AddRange(Directory.GetDirectories(currentPath));
            List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();

            // TODO: get temp directory name
            string tempDirectoryName = "temp";

            allFiles.Sort(new FileNameComparator());
            allFiles.Sort(new FileDateComparator());

            foreach (string file in allFiles)
            {
                FileInfo fileInfo = new FileInfo(file);
                // check if current file/folder is hidden
                if (!(tempDirectoryName.Equals(Path.GetFileName(file)) ||
                    fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                    fileInfo.Name.StartsWith(".") ||
                    Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.GetMetadataConfiguration().GetFilesDirectory()))))
                {
                    FileDescriptionEntity fileDescription = new FileDescriptionEntity();
                    fileDescription.guid = Path.GetFullPath(file);
                    fileDescription.name = Path.GetFileName(file);
                    // set is directory true/false
                    fileDescription.isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory);
                    // set file size
                    if (!fileDescription.isDirectory)
                    {
                        fileDescription.size = fileInfo.Length;
                    }
                    // add object to array list
                    fileList.Add(fileDescription);
                }
            }

            return fileList;
        }

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document info object</returns>
        [HttpPost]
        [Route("metadata/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(MetadataPostedData postedData)
        {
            string password = "";
            try
            {
                LoadDocumentEntity loadDocumentEntity = LoadDocument(postedData);
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, loadDocumentEntity);
            }
            catch (DocumentProtectedException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(ex, password));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Download currently viewed document
        /// </summary>
        /// <param name="path">Path of the document to download</param>
        /// <returns>Document stream as attachment</returns>
        [HttpGet]
        [Route("metadata/downloadDocument")]
        public HttpResponseMessage DownloadDocument(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(path, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(path);
                    return response;
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Upload document
        /// </summary>
        /// <returns>Uploaded document object</returns>
        [HttpPost]
        [Route("metadata/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.GetMetadataConfiguration().GetFilesDirectory();
                bool rewrite = bool.Parse(HttpContext.Current.Request.Form["rewrite"]);
                string fileSavePath = "";
                if (string.IsNullOrEmpty(url))
                {
                    if (HttpContext.Current.Request.Files.AllKeys != null)
                    {
                        // Get the uploaded document from the Files collection
                        var httpPostedFile = HttpContext.Current.Request.Files["file"];
                        if (httpPostedFile != null)
                        {
                            if (rewrite)
                            {
                                // Get the complete file path
                                fileSavePath = Path.Combine(documentStoragePath, httpPostedFile.FileName);
                            }
                            else
                            {
                                fileSavePath = Resources.GetFreeFileName(documentStoragePath, httpPostedFile.FileName);
                            }

                            // Save the uploaded file to "UploadedFiles" folder
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                    }
                }
                else
                {
                    using (WebClient client = new WebClient())
                    {
                        // get file name from the URL
                        Uri uri = new Uri(url);
                        string fileName = Path.GetFileName(uri.LocalPath);
                        if (rewrite)
                        {
                            // Get the complete file path
                            fileSavePath = Path.Combine(documentStoragePath, fileName);
                        }
                        else
                        {
                            fileSavePath = Resources.GetFreeFileName(documentStoragePath, fileName);
                        }
                        // Download the Web resource and save it into the current filesystem folder.
                        client.DownloadFile(url, fileSavePath);
                    }
                }

                UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity();
                uploadedDocument.guid = fileSavePath;

                return Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        private LoadDocumentEntity LoadDocument(MetadataPostedData postedData)
        {
            // get/set parameters
            string documentGuid = postedData.guid;
            string password = string.IsNullOrEmpty(postedData.password) ? null : postedData.password;
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();

            // check if documentGuid contains path or only file name
            if (!Path.IsPathRooted(documentGuid))
            {
                documentGuid = globalConfiguration.GetMetadataConfiguration().GetFilesDirectory() + "/" + documentGuid;
            }

            // set password for protected document
            var loadOptions = new LoadOptions
            {
                Password = password
            };

            using (GroupDocs.Metadata.Metadata metadata = new GroupDocs.Metadata.Metadata(postedData.guid, loadOptions))
            {
                GroupDocs.Metadata.Common.IReadOnlyList<PageInfo> pages = metadata.GetDocumentInfo().Pages;

                using (MemoryStream stream = new MemoryStream())
                {
                    PreviewOptions previewOptions = new PreviewOptions(pageNumber => stream, (pageNumber, pageStream) => { });
                    previewOptions.PreviewFormat = PreviewOptions.PreviewFormats.PNG;

                    for (int i = 0; i < pages.Count; i++)
                    {
                        previewOptions.PageNumbers = new[] { i + 1 };
                        try
                        {
                            metadata.GeneratePreview(previewOptions);
                        }
                        catch (NotSupportedException)
                        {
                            continue;
                        }

                        PageDescriptionEntity pageData = GetPageDescriptionEntities(pages[i]);
                        string encodedImage = Convert.ToBase64String(stream.ToArray());
                        pageData.SetData(encodedImage);
                        loadDocumentEntity.SetPages(pageData);
                        stream.SetLength(0);
                    }
                }
            }

            loadDocumentEntity.SetGuid(documentGuid);

            // return document description
            return loadDocumentEntity;
        }

        private static PageDescriptionEntity GetPageDescriptionEntities(PageInfo page)
        {
            PageDescriptionEntity pageDescriptionEntity = new PageDescriptionEntity();
            pageDescriptionEntity.number = page.PageNumber;
            pageDescriptionEntity.height = page.Height;
            pageDescriptionEntity.width = page.Width;
            return pageDescriptionEntity;
        }

        private static string GetTempPath(MetadataPostedData postedData)
        {
            string tempFilename = Path.GetFileNameWithoutExtension(postedData.guid) + "_tmp";
            return Path.Combine(Path.GetDirectoryName(postedData.guid), tempFilename + Path.GetExtension(postedData.guid));
        }

        private static LoadOptions GetLoadOptions(MetadataPostedData postedData)
        {
            string password = (string.IsNullOrEmpty(postedData.password)) ? null : postedData.password;
            // set password for protected document
            var loadOptions = new LoadOptions
            {
                Password = password
            };

            return loadOptions;
        }
    }
}