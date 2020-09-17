using GroupDocs.Metadata.Exceptions;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Metadata.Config;
using GroupDocs.Total.MVC.Products.Metadata.DTO;
using GroupDocs.Total.MVC.Products.Metadata.Services;
using System;
using System.IO;
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

        private readonly MetadataService metadataService;

        private readonly FileService fileService;

        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataApiController()
        {
            // Check if filesDirectory is relative or absolute path
            globalConfiguration = new Common.Config.GlobalConfiguration();
            metadataService = new MetadataService();
            fileService = new FileService();
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
        public HttpResponseMessage LoadFileTree()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, fileService.LoadFileTree(globalConfiguration));
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
        public HttpResponseMessage LoadProperties(PostedDataDto postedData)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, metadataService.GetPackages(postedData));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Save file properties
        /// </summary>
        /// <param name="postedData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("metadata/saveProperty")]
        public HttpResponseMessage SaveProperties(PostedDataDto postedData)
        {
            try
            {
                metadataService.SaveProperties(postedData);
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
        public HttpResponseMessage RemoveProperty(PostedDataDto postedData)
        {
            try
            {
                metadataService.RemoveProperties(postedData);
                return Request.CreateResponse(HttpStatusCode.OK, new object());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document info object</returns>
        [HttpPost]
        [Route("metadata/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(PostedDataDto postedData)
        {
            string password = "";
            try
            {
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, fileService.LoadDocument(postedData, globalConfiguration));
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
                return Request.CreateResponse(HttpStatusCode.OK, fileService.UploadDocument(HttpContext.Current.Request, globalConfiguration));
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }
    }
}