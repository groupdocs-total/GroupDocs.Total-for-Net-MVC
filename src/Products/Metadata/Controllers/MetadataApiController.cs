using GroupDocs.Metadata;
using GroupDocs.Metadata.Preview;
using GroupDocs.Metadata.Tools;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Metadata.Entity.Web;
using GroupDocs.Total.MVC.Products.Metadata.Loader;
using GroupDocs.Total.MVC.Products.Metadata.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace GroupDocs.Total.MVC.Products.Metadata.Controllers
{
    /// <summary>
    /// MetadataApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MetadataApiController : ApiController
    {
        private static Common.Config.GlobalConfiguration GlobalConfiguration;
        private List<string> SupportedImageFormats = new List<string>() { ".bmp", ".jpeg", ".jpg", ".tiff", ".tif", ".png" };
        private static string storagePath;

        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataApiController()
        {
            // get global configurations 
            GlobalConfiguration = new Common.Config.GlobalConfiguration();
            storagePath = GlobalConfiguration.Metadata.FilesDirectory;
        }


        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">MetadataPostedDataEntity</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("metadata/loadFileTree")]
        public HttpResponseMessage LoadFileTree(MetadataPostedDataEntity postedData)
        {
            string relDirPath = postedData.path;
            try
            {
                // get all the files from a directory
                if (String.IsNullOrEmpty(relDirPath))
                {
                    relDirPath = storagePath;
                }
                else
                {
                    relDirPath = Path.Combine(storagePath, relDirPath);
                }

                List<MetadataFileDescriptionEntity> fileList;
                fileList = new FilesTreeLoader().LoadFiles(relDirPath);

                return Request.CreateResponse(HttpStatusCode.OK, fileList);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">MetadataPostedDataEntity</param>
        /// <returns>All info about the document</returns>
        [HttpPost]
        [Route("metadata/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(MetadataPostedDataEntity postedData)
        {
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                DocumentType docType;
                List<DocumentMetadataDescription> pagesDescription = new List<DocumentMetadataDescription>();
                using (Stream document = File.Open(documentGuid, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (FileFormatChecker fileFormatChecker = new FileFormatChecker(document))
                    {
                        docType = fileFormatChecker.GetDocumentType();
                    }

                    using (PreviewHandler handler = PreviewFactory.Load(document))
                    {
                        bool isSupported = handler.PreviewSupported;
                        PreviewPage[] pages = handler.Pages;
                        // get info about each document page
                        for (int i = 0; i < pages.Length; i++)
                        {
                            //initiate custom Document description object
                            DocumentMetadataDescription description = new DocumentMetadataDescription();
                            // set current page info for result
                            description.height = (handler.UnitOfMeasurement == PreviewUnitOfMeasurement.Point) ? pages[i].Height * 0.75 : pages[i].Height;
                            description.width = (handler.UnitOfMeasurement == PreviewUnitOfMeasurement.Point) ? pages[i].Width * 0.75 : pages[i].Width;
                            description.number = i + 1;
                            pagesDescription.Add(description);
                        }
                    }
                }
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, pagesDescription);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load document page
        /// </summary>
        /// <param name="postedData">MetadataPostedDataEntity</param>
        /// <returns>Document page image encoded in Base64</returns>
        [HttpPost]
        [Route("metadata/loadDocumentPage")]
        public HttpResponseMessage LoadDocumentPage(MetadataPostedDataEntity postedData)
        {
            string encodedImage;
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                int pageNumber = postedData.page;
                string password = postedData.password;
                LoadedPageEntity loadedPage = new LoadedPageEntity();
                using (Stream document = File.Open(documentGuid, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (PreviewHandler handler = PreviewFactory.Load(document))
                    {
                        // get page image
                        if (handler.GetPageImage(pageNumber - 1).Length > 1)
                        {
                            Stream[] images = new MemoryStream[handler.GetPageImage(pageNumber - 1).Length];
                            for (int i = 0; i < handler.GetPageImage(pageNumber - 1).Length; i++)
                            {
                                images[i] = new MemoryStream(handler.GetPageImage(pageNumber - 1)[i].Contents);
                            }
                            encodedImage = CombineBitmap(images);
                        }
                        else
                        {
                            byte[] bytes = handler.GetPageImage(pageNumber - 1)[0].Contents;
                            // encode ByteArray into string
                            encodedImage = Convert.ToBase64String(bytes);
                        }

                        loadedPage.pageImage = encodedImage;
                    }
                }

                // return loaded page object
                return Request.CreateResponse(HttpStatusCode.OK, loadedPage);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load document metadata
        /// </summary>
        /// <param name="postedData">MetadataPostedDataEntity</param>
        /// <returns>All info about the document</returns>
        [HttpPost]
        [Route("metadata/getMetadata")]
        public HttpResponseMessage GetMetadata(MetadataPostedDataEntity postedData)
        {
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                DocumentType docType;
                string metadata;
                using (Stream document = File.Open(documentGuid, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (FileFormatChecker fileFormatChecker = new FileFormatChecker(document))
                    {
                        docType = fileFormatChecker.GetDocumentType();
                    }

                    metadata = MetadataFactory.CreateMetadataImporter(document, docType).ImportMetadata();
                }
                DocumentMetadataDescription documentMetadata = new DocumentMetadataDescription();
                documentMetadata.documentMetadata = metadata;
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, documentMetadata);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Download document
        /// </summary>
        /// <param name="path">string</param>
        /// <param name="signed">bool</param>
        /// <returns></returns>
        [HttpGet]
        [Route("metadata/downloadDocument")]
        public HttpResponseMessage DownloadDocument(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string pathToDownload = "";
                string fileName = Path.GetFileName(path);
                // check if file exists
                if (System.IO.File.Exists(path))
                {
                    // prepare response message
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    pathToDownload = Path.Combine(storagePath, fileName);
                    // add file into the response
                    var fileStream = new FileStream(path, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = fileName;
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
                // get posted data
                string url = HttpContext.Current.Request.Form["url"];
                string metadataType = HttpContext.Current.Request.Form["metadataType"];
                bool rewrite = bool.Parse(HttpContext.Current.Request.Form["rewrite"]);
                // check if file selected or URL
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
                                fileSavePath = Path.Combine(storagePath, httpPostedFile.FileName);
                            }
                            else
                            {
                                fileSavePath = new Resources().GetFreeFileName(storagePath, httpPostedFile.FileName);
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
                            fileSavePath = Path.Combine(fileSavePath, fileName);
                        }
                        else
                        {
                            fileSavePath = new Resources().GetFreeFileName(fileSavePath, fileName);
                        }
                        // Download the Web resource and save it into the current filesystem folder.
                        client.DownloadFile(url, fileSavePath);
                    }
                }
                // initiate uploaded file description class
                MetadataFileDescriptionEntity uploadedDocument = new MetadataFileDescriptionEntity();
                uploadedDocument.guid = fileSavePath;
                MemoryStream ms = new MemoryStream();
                using (FileStream file = new FileStream(fileSavePath, FileMode.Open, FileAccess.Read))
                {
                    file.CopyTo(ms);
                    byte[] imageBytes = ms.ToArray();
                    // Convert byte[] to Base64 String
                    uploadedDocument.image = Convert.ToBase64String(imageBytes);
                }
                ms.Close();
                return Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Combine several images into the one Base64 string
        /// </summary>
        /// <param name="files">Stream[]</param>
        /// <returns>string</returns>
        private static string CombineBitmap(Stream[] files)
        {
            //read all images into memory
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;
            string sigBase64;
            try
            {
                int width = 0;
                int height = 0;

                foreach (Stream image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image);

                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image,
                          new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                using (var ms = new MemoryStream())
                {
                    finalImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    sigBase64 = Convert.ToBase64String(ms.GetBuffer()); //Get Base64                    
                }
                return sigBase64;
            }
            catch (Exception)
            {
                if (finalImage != null)
                    finalImage.Dispose();
                //throw ex;
                throw;
            }
            finally
            {
                //clean up memory
                foreach (System.Drawing.Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }
    }
}