using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Options;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Metadata.Config;
using GroupDocs.Total.MVC.Products.Metadata.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace GroupDocs.Total.MVC.Products.Metadata.Services
{
    public class PreviewService
    {
        private readonly MetadataConfiguration metadataConfiguration;

        private readonly FileService fileService;

        public PreviewService(MetadataConfiguration metadataConfiguration, FileService fileService)
        {
            this.metadataConfiguration = metadataConfiguration;
            this.fileService = fileService;
        }

        public IEnumerable<FileDescriptionEntity> LoadFileTree()
        {
            List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();
            if (!string.IsNullOrEmpty(metadataConfiguration.GetFilesDirectory()))
            {
                var currentPath = metadataConfiguration.GetFilesDirectory();
                var inputDirectory = new DirectoryInfo(currentPath);
                var allFiles = inputDirectory.GetFiles();

                Array.Sort(allFiles, (x, y) => DateTime.Compare(y.CreationTime, x.CreationTime));

                foreach (var file in allFiles)
                {
                    // check if current file/folder is hidden
                    if (!file.Attributes.HasFlag(FileAttributes.Hidden) &&
                        !file.Name.StartsWith(".") &&
                        !file.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        // add object to array list
                        fileList.Add(new FileDescriptionEntity
                        {
                            guid = file.Name,
                            name = file.Name,
                            size = file.Length,
                        });
                    }
                }
            }
            return fileList;
        }

        public DocumentPreviewDto LoadDocument(PostedDataDto postedData)
        {
            string password = string.IsNullOrEmpty(postedData.password) ? null : postedData.password;
            var documentPreview = new DocumentPreviewDto();

            // set password for protected document
            var loadOptions = new LoadOptions
            {
                Password = password
            };

            using (Stream fileStream = fileService.GetSourceFileStream(postedData.guid))
            using (GroupDocs.Metadata.Metadata metadata = new GroupDocs.Metadata.Metadata(fileStream, loadOptions))
            {
                GroupDocs.Metadata.Common.IReadOnlyList<PageInfo> pages = metadata.GetDocumentInfo().Pages;

                using (MemoryStream stream = new MemoryStream())
                {
                    PreviewOptions previewOptions = new PreviewOptions(pageNumber => stream, (pageNumber, pageStream) => { });
                    previewOptions.PreviewFormat = PreviewOptions.PreviewFormats.PNG;

                    int pageCount = pages.Count;
                    if (metadataConfiguration.GetPreloadPageCount() > 0)
                    {
                        pageCount = metadataConfiguration.GetPreloadPageCount();
                    }

                    bool completed = ExecuteWithTimeLimit(TimeSpan.FromMilliseconds(metadataConfiguration.GetPreviewTimeLimit()), () =>
                    {
                        for (int i = 0; i < pageCount; i++)
                        {
                            previewOptions.PageNumbers = new[] { i + 1 };
                            try
                            {
                                metadata.GeneratePreview(previewOptions);
                            }
                            catch (NotSupportedException)
                            {
                                break;
                            }

                            PageDescriptionEntity pageData = GetPageDescriptionEntities(pages[i]);
                            string encodedImage = Convert.ToBase64String(stream.ToArray());
                            pageData.SetData(encodedImage);
                            documentPreview.SetPages(pageData);
                            stream.SetLength(0);
                        }
                    });

                    documentPreview.SetTimeLimitExceeded(!completed);
                }
            }

            documentPreview.SetGuid(postedData.guid);

            // return document description
            return documentPreview;
        }

        public UploadedDocumentEntity UploadDocument(HttpRequest request)
        {
            string url = request.Form["url"];
            // get documents storage path
            string documentStoragePath = metadataConfiguration.GetFilesDirectory();
            bool rewrite = bool.Parse(request.Form["rewrite"]);
            string fileSavePath = string.Empty;
            if (string.IsNullOrEmpty(url))
            {
                // Get the uploaded document from the Files collection
                var httpPostedFile = request.Files["file"];
                if (httpPostedFile == null || Path.IsPathRooted(httpPostedFile.FileName))
                {
                    throw new ArgumentException("Could not upload the file");
                }
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
            uploadedDocument.guid = Path.GetFileName(fileSavePath);

            return uploadedDocument;
        }

        private bool ExecuteWithTimeLimit(TimeSpan timeSpan, Action codeBlock)
        {
            try
            {
                Task task = Task.Factory.StartNew(() => codeBlock());
                task.Wait(timeSpan);
                return task.IsCompleted;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerExceptions[0];
            }
        }

        private PageDescriptionEntity GetPageDescriptionEntities(PageInfo page)
        {
            PageDescriptionEntity pageDescriptionEntity = new PageDescriptionEntity();
            pageDescriptionEntity.number = page.PageNumber;
            pageDescriptionEntity.height = page.Height;
            pageDescriptionEntity.width = page.Width;
            return pageDescriptionEntity;
        }
    }
}