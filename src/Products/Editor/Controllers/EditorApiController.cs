﻿using GroupDocs.Editor.Options;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Total.MVC.Products.Editor.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using GroupDocs.Total.MVC.Products.Editor.Entity.Web.Request;
using GroupDocs.Editor;
using GroupDocs.Editor.Formats;

namespace GroupDocs.Total.MVC.Products.Editor.Controllers
{
    /// <summary>
    /// EditorApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EditorApiController : ApiController
    {

        private static Common.Config.GlobalConfiguration globalConfiguration = new Common.Config.GlobalConfiguration();

        /// <summary>
        /// Load Viewr configuration
        /// </summary>       
        /// <returns>Editor configuration</returns>
        [HttpGet]
        [Route("editor/loadConfig")]
        public EditorConfiguration LoadConfig()
        {
            return globalConfiguration.GetEditorConfiguration();
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("editor/loadFileTree")]
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
                    relDirPath = globalConfiguration.GetEditorConfiguration().GetFilesDirectory();
                }
                else
                {
                    relDirPath = Path.Combine(globalConfiguration.GetEditorConfiguration().GetFilesDirectory(), relDirPath);
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
                    if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.GetEditorConfiguration().GetFilesDirectory())) ||
                        Path.GetFileName(file).Equals(".gitkeep"))
                    {
                        // ignore current file and skip to next one
                        continue;
                    }
                    else
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
        /// Load supported file types
        /// </summary>       
        /// <returns>Editor configuration</returns>
        [HttpGet]
        [Route("editor/loadFormats")]
        public List<string> LoadFormats()
        {
            return PrepareFormats();
        }

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document info object</returns>
        [HttpPost]
        [Route("editor/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(PostedDataEntity postedData)
        {
            try
            {
                LoadDocumentEntity loadDocumentEntity = LoadDocument(postedData.guid, postedData.password);
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, loadDocumentEntity);
            }
            catch (PasswordRequiredException ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(ex, postedData.password));
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex, postedData.password));
            }
        }

        /// <summary>
        /// Download curerntly viewed document
        /// </summary>
        /// <param name="path">Path of the document to download</param>
        /// <returns>Document stream as attachement</returns>
        [HttpGet]
        [Route("editor/downloadDocument")]
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
        /// <param name="postedData">Post data</param>
        /// <returns>Uploaded document object</returns>
        [HttpPost]
        [Route("editor/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.GetEditorConfiguration().GetFilesDirectory();
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

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document info object</returns>
        [HttpPost]
        [Route("editor/saveFile")]
        public HttpResponseMessage SaveFile(EditDocumentRequest postedData)
        {
            try
            {
                string htmlContent = postedData.getContent(); // Initialize with HTML markup of the edited document
                string saveFilePath = Path.Combine(globalConfiguration.GetEditorConfiguration().GetFilesDirectory(), postedData.GetGuid());

                string tempFilename = Path.GetFileNameWithoutExtension(saveFilePath) + ".tmp";
                string tempPath = Path.Combine(Path.GetDirectoryName(saveFilePath), tempFilename);

                using (GroupDocs.Editor.Editor editor = new GroupDocs.Editor.Editor(postedData.GetGuid()))
                {
                    WordProcessingEditOptions editOptions = new WordProcessingEditOptions();
                    editOptions.FontExtraction = FontExtractionOptions.ExtractEmbeddedWithoutSystem;
                    editOptions.EnableLanguageInformation = true;
                    editOptions.EnablePagination = true;
                    using (EditableDocument beforeEdit = editor.Edit(editOptions))
                    {
                        EditableDocument htmlContentDoc = EditableDocument.FromMarkup(htmlContent, null);
                        WordProcessingSaveOptions saveOptions = new WordProcessingSaveOptions(WordProcessingFormats.Docm);
                        saveOptions.EnablePagination = true;

                        using (FileStream outputStream = File.Create(tempPath))
                        {
                            editor.Save(htmlContentDoc, outputStream, saveOptions);
                        }
                    }
                }

                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                }

                File.Move(tempPath, saveFilePath);

                LoadDocumentEntity loadDocumentEntity = LoadDocument(saveFilePath, postedData.getPassword());
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, loadDocumentEntity);
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex, postedData.getPassword()));
            }
        }

        private static ILoadOptions GetLoadOptions(string guid)
        {
            string extension = Path.GetExtension(guid).Replace(".", "");
            extension = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(extension);

            if (extension.Equals("Txt"))
            {
                extension = "Text";
            }

            ILoadOptions options = null;

            foreach (var item in typeof(WordProcessingFormats).GetFields())
            {
                if (item.Name.Equals("Auto"))
                {
                    continue;
                }

                if (item.Name.Equals(extension))
                {
                    options = new WordProcessingLoadOptions();
                    break;
                }
            }

            if (options == null)
            {
                options = new SpreadsheetLoadOptions();
            }

            return options;
        }

        private static List<string> PrepareFormats()
        {
            List<string> outputListItems = new List<string>();

            foreach (var item in typeof(WordProcessingFormats).GetFields())
            {
                if (item.Name.Equals("Auto"))
                {
                    continue;
                }

                if (item.Name.Equals("Text"))
                {
                    outputListItems.Add("Txt");
                }

                else
                {
                    outputListItems.Add(item.Name);
                }
            }

            foreach (var item in typeof(SpreadsheetFormats).GetFields())
            {
                if (item.Name.Equals("Auto"))
                {
                    continue;
                }

                outputListItems.Add(item.Name);
            }

            return outputListItems;
        }

        private LoadDocumentEntity LoadDocument(string guid, string password)
        {
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();
            ILoadOptions loadOptions = GetLoadOptions(guid);
            loadOptions.Password = password;

            // Instantiate Editor object by loading the input file
            using (GroupDocs.Editor.Editor editor = new GroupDocs.Editor.Editor(guid, delegate { return loadOptions; }))
            {
                WordProcessingEditOptions editOptions = new WordProcessingEditOptions();
                editOptions.EnablePagination = true;

                // Open input document for edit — obtain an intermediate document, that can be edited
                EditableDocument beforeEdit = editor.Edit(editOptions);

                // Get document as a single base64-encoded string, where all resources (images, fonts, etc) 
                // are embedded inside this string along with main textual content
                string allEmbeddedInsideString = beforeEdit.GetEmbeddedHtml();

                loadDocumentEntity.SetGuid(guid);
                PageDescriptionEntity page = new PageDescriptionEntity();
                page.SetData(allEmbeddedInsideString);
                loadDocumentEntity.SetPages(page);

                beforeEdit.Dispose();
            }

            return loadDocumentEntity;
        }
    }
}