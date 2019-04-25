using GroupDocs.Conversion.Config;
using GroupDocs.Conversion.Handler;
using GroupDocs.Conversion.Options.Save;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Total.MVC.Products.Conversion.Entity.Web.Request;
using GroupDocs.Total.MVC.Products.Conversion.Entity.Web.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GroupDocs.Total.MVC.Products.Conversion.Controllers
{
    /// <summary>
    /// ViewerApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ConversionApiController : ApiController
    {

        private static Common.Config.GlobalConfiguration globalConfiguration;
        private static ConversionHandler conversionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConversionApiController()
        {
            // Check if filesDirectory is relative or absolute path           
            globalConfiguration = new Common.Config.GlobalConfiguration();
            // Setup Conversion configuration
            var conversionConfig = new ConversionConfig
            {
                StoragePath = globalConfiguration.Conversion.GetFilesDirectory(),
                OutputPath = globalConfiguration.Conversion.GetResultDirectory()
            };
            conversionHandler = new ConversionHandler(conversionConfig);
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("conversion/loadFileTree")]
        public HttpResponseMessage loadFileTree(PostedDataEntity postedData)
        {
            // get request body       
            string relDirPath = postedData.path;
            // get file list from storage path
            try
            {
                // get all the files from a directory
                if (string.IsNullOrEmpty(relDirPath))
                {
                    relDirPath = globalConfiguration.Conversion.GetFilesDirectory();
                }
                else
                {
                    relDirPath = Path.Combine(globalConfiguration.Conversion.GetFilesDirectory(), relDirPath);
                }

                List<string> allFiles = new List<string>(Directory.GetFiles(relDirPath));
                allFiles.AddRange(Directory.GetDirectories(relDirPath));
                List<ConversionTypesEntity> fileList = new List<ConversionTypesEntity>();

                allFiles.Sort(new FileNameComparator());
                allFiles.Sort(new FileTypeComparator());

                foreach (string file in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // check if current file/folder is hidden
                    if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.Conversion.GetFilesDirectory())))
                    {
                        // ignore current file and skip to next one
                        continue;
                    }
                    else
                    {
                        ConversionTypesEntity fileDescription = new ConversionTypesEntity();
                        fileDescription.conversionTypes = new List<string>();
                        fileDescription.guid = Path.GetFullPath(file);
                        fileDescription.name = Path.GetFileName(file);
                        // set is directory true/false
                        fileDescription.isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory);
                        // set file size
                        if (!fileDescription.isDirectory)
                        {
                            fileDescription.size = fileInfo.Length;
                        }

                        string documentExtension = Path.GetExtension(fileDescription.name).TrimStart('.');
                        if (!String.IsNullOrEmpty(documentExtension))
                        {
                            string[] availableConversions = conversionHandler.GetPossibleConversions(documentExtension);
                            //list all available conversions
                            foreach (string name in availableConversions)
                            {
                                fileDescription.conversionTypes.Add(name);
                            }
                        }
                        // add object to array list
                        fileList.Add(fileDescription);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, fileList);
            }
            catch (System.Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Upload document
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Uploaded document object</returns>
        [HttpPost]
        [Route("conversion/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.Conversion.GetFilesDirectory();
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
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("conversion/convert")]
        public HttpResponseMessage Convert(ConversionPostedData postedData)
        {
            try
            {
                string sourceType = Path.GetExtension(postedData.guid).TrimStart('.');
                string destinationType = postedData.GetDestinationType();
                string resultFileName = Path.GetFileNameWithoutExtension(postedData.guid) + "." + postedData.GetDestinationType();
                dynamic saveOptions = GetSaveOptions(sourceType, destinationType, postedData.password);
                ConvertedDocument convertedDocument = conversionHandler.Convert(postedData.guid, saveOptions);
                if (convertedDocument.PageCount > 1 && saveOptions is ImageSaveOptions)
                {
                    for(int i = 1; i <= convertedDocument.PageCount; i++)
                    {                        
                        string fileName = Path.GetFileNameWithoutExtension(resultFileName) + "-page" + i + "." + Path.GetExtension(resultFileName);
                        convertedDocument.Save(fileName, i);
                    }
                }
                else
                {
                    convertedDocument.Save(resultFileName);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        private SaveOptions GetSaveOptions(string sourceType, string destinationType, string password)
        {
            dynamic saveOptions = null;
            Dictionary<string, SaveOptions> availableConversions = conversionHandler.GetSaveOptions(sourceType);
            //list all available conversions
            foreach (var conversion in availableConversions)
            {
                if (conversion.Key.Equals(destinationType))
                {
                    saveOptions = conversion.Value;
                }
            }
            return saveOptions;
        }
    }
}
