using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using GroupDocs.Total.MVC.Products.Search.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Search;
using GroupDocs.Search.Options;
using GroupDocs.Total.MVC.Products.Search.Entity.Web.Request;
using GroupDocs.Total.MVC.Products.Search.Service;
using GroupDocs.Search.Common;
using GroupDocs.Total.MVC.Products.Search.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Search.Controllers
{
    /// <summary>
    /// SearchApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SearchApiController : ApiController
    {
        private readonly Common.Config.GlobalConfiguration globalConfiguration;
        private readonly Index index;
        private readonly Dictionary<string, DocumentStatus> filesIndexStatuses = new Dictionary<string, DocumentStatus>();

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchApiController()
        {
            globalConfiguration = new Common.Config.GlobalConfiguration();

            index = new Index();

            index.Events.OperationProgressChanged += (sender, args) =>
            {
                filesIndexStatuses.Add(args.LastDocumentPath, args.LastDocumentStatus);
            };

            index.Add(globalConfiguration.GetSearchConfiguration().GetFilesDirectory());
        }

        /// <summary>
        /// Load Search configuration
        /// </summary>
        /// <returns>Search configuration</returns>
        [HttpGet]
        [Route("search/loadConfig")]
        public SearchConfiguration LoadConfig()
        {
            return globalConfiguration.GetSearchConfiguration();
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("search/loadFileTree")]
        public HttpResponseMessage LoadFileTree()
        {
            try
            {
                List<IndexedFileDescriptionEntity> filesList = new List<IndexedFileDescriptionEntity>();
                if (!string.IsNullOrEmpty(globalConfiguration.GetSearchConfiguration().GetFilesDirectory()))
                {
                    filesList = LoadFiles();
                }
                return Request.CreateResponse(HttpStatusCode.OK, filesList);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load documents
        /// </summary>
        /// <returns>List[FileDescriptionEntity]</returns>
        public List<IndexedFileDescriptionEntity> LoadFiles()
        {
            var currentPath = globalConfiguration.GetSearchConfiguration().GetFilesDirectory();

            List<string> allFiles = new List<string>(Directory.GetFiles(currentPath));
            allFiles.AddRange(Directory.GetDirectories(currentPath));
            List<IndexedFileDescriptionEntity> fileList = new List<IndexedFileDescriptionEntity>();

            allFiles.Sort(new FileNameComparator());
            allFiles.Sort(new FileDateComparator());

            foreach (string file in allFiles)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (!(Path.GetFileName(file).StartsWith(".") ||
                      fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                      Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.GetSearchConfiguration().GetFilesDirectory()))))
                {
                    IndexedFileDescriptionEntity fileDescription = new IndexedFileDescriptionEntity
                    {
                        guid = Path.GetFullPath(file),
                        name = Path.GetFileName(file),
                        // set is directory true/false
                        isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory)
                    };
                    // set file size
                    if (!fileDescription.isDirectory)
                    {
                        fileDescription.size = fileInfo.Length;
                    }

                    DocumentStatus value;
                    if (filesIndexStatuses.TryGetValue(fileDescription.guid, out value))
                    {
                        fileDescription.documentStatus = value;
                    }

                    fileList.Add(fileDescription);
                }
            }

            return fileList;
        }

        /// <summary>
        /// Upload document
        /// </summary>
        /// <returns>Uploaded document object</returns>
        [HttpPost]
        [Route("search/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.GetSearchConfiguration().GetFilesDirectory();
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

                UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity
                {
                    guid = fileSavePath
                };

                UpdateOptions options = new UpdateOptions
                {
                    Threads = 2
                };

                index.Update(options);

                return Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Performs search
        /// </summary>
        /// <param name="postedData">SearchPostedData</param>
        /// <returns>Search results</returns>
        [HttpPost]
        [Route("search/search")]
        public HttpResponseMessage Search(SearchPostedData postedData)
        {
            try
            {
                var result = SearchService.Search(index, postedData, globalConfiguration);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="postedData">PostedDataEntity</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        [Route("search/deleteFile")]
        public HttpResponseMessage DeleteFile(PostedDataEntity postedData)
        {
            try
            {
                if (File.Exists(postedData.guid))
                {
                    File.Delete(postedData.guid);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }
    }
}