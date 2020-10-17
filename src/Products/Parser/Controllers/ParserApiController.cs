using GroupDocs.Total.MVC.Products.Parser.Config;
using GroupDocs.Total.MVC.Products.Parser.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Helpers;
using GroupDocs.Parser.Templates;
using System.Text.RegularExpressions;
using GroupDocs.Parser.Data;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Parser.Controllers
{
    /// <summary>
    /// Parser Api Controller
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ParserApiController : ApiController
    {
        private static Common.Config.GlobalConfiguration globalConfiguration = new Common.Config.GlobalConfiguration();

        /// <summary>
        /// Load Parser configuration
        /// </summary>
        /// <returns>Parser configuration</returns>
        [HttpGet]
        [Route("parser/loadConfig")]
        public ParserConfiguration LoadConfig()
        {
            return globalConfiguration.GetParserConfiguration();
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("parser/loadFileTree")]
        public HttpResponseMessage LoadFileTree(PostedDataEntity postedData)
        {
            // get request body
            string relDirPath = postedData.path;
            // get file list from storage path
            try
            {
                // get all the files from a directory
                if (string.IsNullOrEmpty(relDirPath))
                {
                    relDirPath = globalConfiguration.GetParserConfiguration().GetFilesDirectory();
                }
                else
                {
                    relDirPath = Path.Combine(globalConfiguration.GetParserConfiguration().GetFilesDirectory(), relDirPath);
                }

                List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();
                List<string> allFiles = new List<string>(Directory.GetFiles(relDirPath));
                allFiles.AddRange(Directory.GetDirectories(relDirPath));

                allFiles.Sort(new FileNameComparator());
                allFiles.Sort(new FileTypeComparator());

                foreach (string file in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // check if current file/folder is hidden
                    if (!(fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.GetParserConfiguration().GetFilesDirectory())) ||
                        Path.GetFileName(file).StartsWith(".")))
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
                return Request.CreateResponse(HttpStatusCode.OK, fileList);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Upload document
        /// </summary>      
        /// <returns>Uploaded document object</returns>
        [HttpPost]
        [Route("parser/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.GetParserConfiguration().GetFilesDirectory();
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
                return Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document info object</returns>
        [HttpPost]
        [Route("parser/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(PostedDataEntity postedData)
        {
            string password = "";
            try
            {
                ParserDocumentEntity loadDocumentEntity = LoadDocument(postedData, globalConfiguration.GetParserConfiguration().GetPreloadPageCount() == 0);
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, loadDocumentEntity);
            }
            catch (GroupDocs.Parser.Exceptions.InvalidPasswordException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(ex, password));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex, password));
            }
        }

        [HttpPost]
        [Route("parser/parseByTemplate")]
        public HttpResponseMessage ParseByTemplate()
        {
            try
            {
                using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
                {
                    dynamic data = System.Web.Helpers.Json.Decode(reader.ReadToEnd());

                    using (GroupDocs.Parser.Parser parser = CreateParser(data.guid, data.password))
                    {
                        var fields = new List<TemplateField>();
                        foreach (dynamic field in data.fields)
                        {
                            fields.Add(new TemplateField(new TemplateFixedPosition(new Rectangle(
                                new Point((double)field.position.x, (double)field.position.y),
                                new Size((double)field.size.width, (double)field.size.height))), field.name, field.pageNumber - 1));
                        }

                        var template = new Template(fields);
                        var parsedData = parser.ParseByTemplate(template)
                            .Select(x => new { name = x.Name, value = (x.PageArea as PageTextArea)?.Text });

                        return Request.CreateResponse(HttpStatusCode.OK, parsedData);
                    }
                }
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        private ParserDocumentEntity LoadDocument(PostedDataEntity loadDocumentRequest, bool loadAllPages)
        {
            string password = loadDocumentRequest.password;
            ParserDocumentEntity description = new ParserDocumentEntity();
            string documentGuid = loadDocumentRequest.guid;

            using (GroupDocs.Parser.Parser parser = CreateParser(documentGuid, password))
            {
                GroupDocs.Parser.Options.IDocumentInfo documentInfo = parser.GetDocumentInfo();
                IList<string> pagesContent = loadAllPages
                    ? GetAllPagesContent(password, documentGuid, documentInfo)
                    : new List<string>();

                for (int i = 0; i < documentInfo.PageCount; i++)
                {
                    PageDescriptionEntity page = new PageDescriptionEntity
                    {
                        number = i + 1,
                        width = documentInfo.Pages[i].Width,
                        height = documentInfo.Pages[i].Height
                    };

                    if (pagesContent.Count > 0)
                    {
                        page.SetData(pagesContent[i]);
                    }

                    description.pages.Add(page);
                }
            }

            description.guid = documentGuid;

            // return document description
            return description;
        }

        private static GroupDocs.Parser.Parser CreateParser(string documentGuid, string password)
        {
            return string.IsNullOrEmpty(password)
                ? new GroupDocs.Parser.Parser(documentGuid)
                : new GroupDocs.Parser.Parser(documentGuid, new GroupDocs.Parser.Options.LoadOptions(password));
        }

        private static GroupDocs.Parser.Parser CreateParser(Stream stream, string password)
        {
            return string.IsNullOrEmpty(password)
                ? new GroupDocs.Parser.Parser(stream)
                : new GroupDocs.Parser.Parser(stream, new GroupDocs.Parser.Options.LoadOptions(password));
        }

        private static List<string> GetAllPagesContent(string password, string documentGuid, GroupDocs.Parser.Options.IDocumentInfo pages)
        {
            List<string> allPages = new List<string>();

            //get page HTML
            for (int i = 0; i < pages.PageCount; i++)
            {
                byte[] bytes;
                using (var memoryStream = RenderPageToMemoryStream(i + 1, documentGuid, password))
                {
                    bytes = memoryStream.ToArray();
                }

                string encodedImage = Convert.ToBase64String(bytes);
                allPages.Add(encodedImage);
            }

            return allPages;
        }

        static MemoryStream RenderPageToMemoryStream(int pageNumberToRender, string documentGuid, string password)
        {
            MemoryStream result = new MemoryStream();

            using (FileStream outputStream = File.OpenRead(documentGuid))
            {
                using (GroupDocs.Parser.Parser parser = CreateParser(outputStream, password))
                {
                    GroupDocs.Parser.Options.PreviewOptions previewOptions = new GroupDocs.Parser.Options.PreviewOptions(pageNumber => result)
                    {
                        PreviewFormat = GroupDocs.Parser.Options.PreviewOptions.PreviewFormats.PNG,
                        PageNumbers = new[] { pageNumberToRender },
                    };

                    parser.GeneratePreview(previewOptions);
                }
            }

            return result;
        }
    }
}