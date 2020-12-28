using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Entity.Web.Request;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Total.MVC.Products.Search.Config;
using GroupDocs.Total.MVC.Products.Search.Entity.Web;
using GroupDocs.Total.MVC.Products.Search.Entity.Web.Request;
using GroupDocs.Total.MVC.Products.Search.Entity.Web.Response;
using GroupDocs.Total.MVC.Products.Search.Service;

namespace GroupDocs.Total.MVC.Products.Search.Controllers
{
    /// <summary>
    /// SearchApiController.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SearchApiController : ApiController
    {
        private readonly Common.Config.GlobalConfiguration globalConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchApiController"/> class.
        /// Constructor.
        /// </summary>
        public SearchApiController()
        {
            this.globalConfiguration = new Common.Config.GlobalConfiguration();
        }

        /// <summary>
        /// Load Search configuration.
        /// </summary>
        /// <returns>Search configuration.</returns>
        [HttpGet]
        [Route("search/loadConfig")]
        public SearchConfiguration LoadConfig()
        {
            SearchService.InitIndex(this.globalConfiguration);

            return this.globalConfiguration.GetSearchConfiguration();
        }

        [HttpPost]
        [Route("search/getUploadedFiles")]
        public IndexedFileDescriptionEntity[] GetUploadedFiles(SearchBaseRequest request)
        {
            var directory = globalConfiguration.GetSearchConfiguration().GetFilesDirectory();
            var files = Directory.GetFiles(directory)
                .Select(filePath =>
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    var descriptor = new IndexedFileDescriptionEntity()
                    {
                        guid = fileInfo.FullName,
                        name = fileInfo.Name,
                        isDirectory = false,
                        size = fileInfo.Length,
                    };
                    return descriptor;
                })
                .ToArray();
            return files;
        }

        [HttpPost]
        [Route("search/getIndexedFiles")]
        public IndexedFileDescriptionEntity[] GetIndexedFiles(SearchBaseRequest request)
        {
            var directory = globalConfiguration.GetSearchConfiguration().GetIndexedFilesDirectory();
            var files = Directory.GetFiles(directory)
                .Select(filePath =>
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    var fullPath = fileInfo.FullName;
                    string status;
                    if (!SearchService.FileIndexingStatusDict.TryGetValue(fullPath, out status))
                    {
                        status = "Indexing";
                    }
                    var descriptor = new IndexedFileDescriptionEntity()
                    {
                        guid = fullPath,
                        name = fileInfo.Name,
                        isDirectory = false,
                        size = fileInfo.Length,
                        documentStatus = status,
                    };
                    return descriptor;
                })
                .ToArray();
            return files;
        }

        /// <summary>
        /// Uploads document.
        /// </summary>
        /// <returns>Uploaded document object.</returns>
        [HttpPost]
        [Route("search/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];

                // get documents storage path
                string documentStoragePath = this.globalConfiguration.GetSearchConfiguration().GetFilesDirectory();
                bool rewrite = this.globalConfiguration.GetSearchConfiguration().rewrite;
                string fileSavePath = string.Empty;
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
                    guid = fileSavePath,
                };

                return this.Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/deleteFiles")]
        public HttpResponseMessage DeleteFiles(FilesDeleteRequest request)
        {
            try
            {
                var documentStoragePath = this.globalConfiguration.GetSearchConfiguration().GetFilesDirectory();
                foreach (var file in request.Files)
                {
                    var fileSavePath = Path.Combine(documentStoragePath, file.guid);
                    File.Delete(fileSavePath);
                }

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Adds files to index.
        /// </summary>
        /// <param name="postedData">Files array.</param>
        /// <returns>HttpResponseMessage.</returns>
        [HttpPost]
        [Route("search/addFilesToIndex")]
        public HttpResponseMessage AddFilesToIndex(AddToIndexRequest request)
        {
            try
            {
                SearchService.AddFilesToIndex(request.Files, this.globalConfiguration);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Performs search.
        /// </summary>
        /// <param name="postedData">Search query.</param>
        /// <returns>Search results.</returns>
        [HttpPost]
        [Route("search/search")]
        public HttpResponseMessage Search(SearchPostedData postedData)
        {
            try
            {
                var result = SearchService.Search(postedData, this.globalConfiguration);
                return this.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Delete file.
        /// </summary>
        /// <param name="postedData">File info.</param>
        /// <returns>HttpResponseMessage.</returns>
        [HttpPost]
        [Route("search/removeFromIndex")]
        public HttpResponseMessage RemoveFromIndex(PostedDataEntity postedData)
        {
            try
            {
                SearchService.RemoveFileFromIndex(postedData.guid);

                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets documents status.
        /// </summary>
        /// <param name="postedData">Files array.</param>
        /// <returns>Indexed files list with current status.</returns>
        [HttpPost]
        [Route("search/getFileStatus")]
        public HttpResponseMessage GetFileStatus(FileStatusGetRequest request)
        {
            var indexingFilesList = new List<IndexedFileDescriptionEntity>();

            foreach (var file in request.Files)
            {
                var indexingFile = new IndexedFileDescriptionEntity();

                string value;
                if (SearchService.FileIndexingStatusDict.TryGetValue(file.guid, out value))
                {
                    if (value.Equals("PasswordRequired"))
                    {
                        return this.Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(new Exception("Password required.")));
                    }

                    indexingFile.documentStatus = value;
                }
                else
                {
                    indexingFile.documentStatus = "Indexing";
                }

                indexingFile.guid = file.guid;

                indexingFilesList.Add(indexingFile);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, indexingFilesList);
        }

        /// <summary>
        /// Gets index properties.
        /// </summary>
        /// <returns>The index properties.</returns>
        [HttpPost]
        [Route("search/getIndexProperties")]
        public HttpResponseMessage GetIndexProperties(SearchBaseRequest request)
        {
            try
            {
                var indexProperties = SearchService.GetIndexProperties();
                return this.Request.CreateResponse(HttpStatusCode.OK, indexProperties);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets the contents of the alphabet dictionary.
        /// </summary>
        /// <returns>The contents of the alphabet dictionary.</returns>
        [HttpPost]
        [Route("search/getAlphabetDictionary")]
        public HttpResponseMessage GetAlphabetDictionary()
        {
            try
            {
                var response = SearchService.GetAlphabetDictionary();
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Updates the contents of the alphabet dictionary.
        /// </summary>
        /// <param name="request">The new contents of the alphabet dictionary.</param>
        /// <returns>HTTP response message.</returns>
        [HttpPost]
        [Route("search/setAlphabetDictionary")]
        public HttpResponseMessage SetAlphabetDictionary(AlphabetUpdateRequest request)
        {
            try
            {
                SearchService.SetAlphabetDictionary(request);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets the contents of the alphabet dictionary.
        /// </summary>
        /// <returns>The contents of the alphabet dictionary.</returns>
        [HttpPost]
        [Route("search/getStopWordDictionary")]
        public HttpResponseMessage GetStopWordDictionary()
        {
            try
            {
                var response = SearchService.GetStopWordDictionary();
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Updates the contents of the stop word dictionary.
        /// </summary>
        /// <param name="request">The new contents of the stop word dictionary.</param>
        /// <returns>HTTP response message.</returns>
        [HttpPost]
        [Route("search/setStopWordDictionary")]
        public HttpResponseMessage SetStopWordDictionary(StopWordsUpdateRequest request)
        {
            try
            {
                SearchService.SetStopWordDictionary(request);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets the contents of the synonym dictionary.
        /// </summary>
        /// <returns>The contents of the synonym dictionary.</returns>
        [HttpPost]
        [Route("search/getSynonymDictionary")]
        public HttpResponseMessage GetSynonymDictionary()
        {
            try
            {
                var response = SearchService.GetSynonymGroups();
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Updates the contents of the synonym dictionary.
        /// </summary>
        /// <param name="request">The new contents of the synonym dictionary.</param>
        /// <returns>HTTP response message.</returns>
        [HttpPost]
        [Route("search/setSynonymDictionary")]
        public HttpResponseMessage SetSynonymDictionary(SynonymsUpdateRequest request)
        {
            try
            {
                SearchService.SetSynonymGroups(request);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets the contents of the homophone dictionary.
        /// </summary>
        /// <returns>The contents of the homophone dictionary.</returns>
        [HttpPost]
        [Route("search/getHomophoneDictionary")]
        public HttpResponseMessage GetHomophoneDictionary()
        {
            try
            {
                var response = SearchService.GetHomophoneGroups();
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Updates the contents of the homophone dictionary.
        /// </summary>
        /// <param name="request">The new contents of the homophone dictionary.</param>
        /// <returns>HTTP response message.</returns>
        [HttpPost]
        [Route("search/setHomophoneDictionary")]
        public HttpResponseMessage SetHomophoneDictionary(HomophonesUpdateRequest request)
        {
            try
            {
                SearchService.SetHomophoneGroups(request);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets the contents of the spelling corrector dictionary.
        /// </summary>
        /// <returns>The contents of the spelling corrector dictionary.</returns>
        [HttpPost]
        [Route("search/getSpellingCorrectorDictionary")]
        public HttpResponseMessage GetSpellingCorrectorDictionary()
        {
            try
            {
                var response = SearchService.GetSpellingCorrectorWords();
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Updates the contents of the spelling corrector dictionary.
        /// </summary>
        /// <param name="request">The new contents of the spelling corrector dictionary.</param>
        /// <returns>HTTP response message.</returns>
        [HttpPost]
        [Route("search/setSpellingCorrectorDictionary")]
        public HttpResponseMessage SetSpellingCorrectorDictionary(SpellingCorrectorUpdateRequest request)
        {
            try
            {
                SearchService.SetSpellingCorrectorWords(request);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets the contents of the character replacement dictionary.
        /// </summary>
        /// <returns>The contents of the character replacement dictionary.</returns>
        [HttpPost]
        [Route("search/getCharacterReplacementDictionary")]
        public HttpResponseMessage GetCharacterReplacementDictionary()
        {
            try
            {
                var response = SearchService.GetCharacterReplacements();
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Updates the contents of the character replacement dictionary.
        /// </summary>
        /// <param name="request">The new contents of the character replacement dictionary.</param>
        /// <returns>HTTP response message.</returns>
        [HttpPost]
        [Route("search/setCharacterReplacementDictionary")]
        public HttpResponseMessage SetCharacterReplacementDictionary(CharacterReplacementsUpdateRequest request)
        {
            try
            {
                SearchService.SetCharacterReplacements(request);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets the contents of the document password dictionary.
        /// </summary>
        /// <returns>The contents of the document password dictionary.</returns>
        [HttpPost]
        [Route("search/getDocumentPasswordDictionary")]
        public HttpResponseMessage GetDocumentPasswordDictionary()
        {
            try
            {
                var response = SearchService.GetDocumentPasswords();
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Updates the contents of the document password dictionary.
        /// </summary>
        /// <param name="request">The new contents of the document password dictionary.</param>
        /// <returns>HTTP response message.</returns>
        [HttpPost]
        [Route("search/setDocumentPasswordDictionary")]
        public HttpResponseMessage SetDocumentPasswordDictionary(DocumentPasswordsUpdateRequest request)
        {
            try
            {
                SearchService.SetDocumentPasswords(request);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Updates the contents of the stop word dictionary.
        /// </summary>
        /// <param name="request">The new contents of the stop word dictionary.</param>
        /// <returns>HTTP response message.</returns>
        [HttpPost]
        [Route("search/highlightTerms")]
        public HttpResponseMessage HighlightTerms(HighlightTermsRequest request)
        {
            try
            {
                var response = SearchService.HighlightTerms(request, this.globalConfiguration.GetSearchConfiguration().GetFilesDirectory());
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }
    }
}
