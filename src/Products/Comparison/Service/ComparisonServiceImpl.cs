using System;
using System.Collections.Generic;
using System.IO;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Comparison.Model.Response;
using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Total.MVC.Products.Comparison.Model.Request;
using GroupDocs.Comparison.Common;
using GroupDocs.Comparison;
using GroupDocs.Comparison.Common.ComparisonSettings;
using GroupDocs.Comparison.Common.Changes;

namespace GroupDocs.Total.MVC.Products.Comparison.Service
{
    public class ComparisonServiceImpl : IComparisonService
    {
        private static readonly string DOCX = ".docx";
        private static readonly string DOC = ".doc";
        private static readonly string XLS = ".xls";
        private static readonly string XLSX = ".xlsx";
        private static readonly string PPT = ".ppt";
        private static readonly string PPTX = ".pptx";
        private static readonly string PDF = ".pdf";
        private static readonly string TXT = ".txt";
        private static readonly string HTML = ".html";
        private static readonly string HTM = ".htm";

        private GlobalConfiguration globalConfiguration;

        public ComparisonServiceImpl(GlobalConfiguration globalConfiguration)
        {
            this.globalConfiguration = globalConfiguration;
        }

        public List<FileDescriptionEntity> LoadFiles(PostedDataEntity fileTreeRequest)
        {
            // get request body       
            string relDirPath = fileTreeRequest.path;
            // get file list from storage path
            try
            {
                // get all the files from a directory
                if (string.IsNullOrEmpty(relDirPath))
                {
                    relDirPath = globalConfiguration.Comparison.GetFilesDirectory();
                }
                else
                {
                    relDirPath = Path.Combine(globalConfiguration.Comparison.GetFilesDirectory(), relDirPath);
                }

                List<string> allFiles = new List<string>(Directory.GetFiles(relDirPath));
                allFiles.AddRange(Directory.GetDirectories(relDirPath));
                List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();

                allFiles.Sort(new FileNameComparator());
                allFiles.Sort(new FileTypeComparator());

                foreach (string file in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // check if current file/folder is hidden
                    if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.Comparison.GetFilesDirectory())))
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
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckFiles(CompareRequest files)
        {
            string extension = Path.GetExtension(files.guids[0].GetGuid());
            // check if files extensions are the same and support format file
            if (!CheckSupportedFiles(extension))
            {
                return false;
            }
            foreach (CompareFileDataRequest path in files.guids)
            {
                if (!extension.Equals(Path.GetExtension(path.GetGuid())))
                {
                    return false;
                }
            }
            return true;
        }

        public CompareResultResponse Compare(CompareRequest compareRequest)
        {
            CompareResultResponse compareResultResponse = new CompareResultResponse();
            if (compareRequest.guids.Count > 2)
            {
                compareResultResponse = MultiCompareFiles(compareRequest);
            }
            else
            {
                compareResultResponse = CompareTwoDocuments(compareRequest);
            }
            return compareResultResponse;
        }

        public LoadDocumentEntity LoadDocumentPages(string path)
        {
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();
            //load file with results
            try
            {
                Comparer comparer = new Comparer();
                List<PageImage> resultImages = comparer.ConvertToImages(path);

                foreach (PageImage page in resultImages)
                {
                    PageDescriptionEntity loadedPage = new PageDescriptionEntity();
                    byte[] bytes = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        page.PageStream.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                    loadedPage.SetData(Convert.ToBase64String(bytes));
                    loadDocumentEntity.SetPages(loadedPage);
                }
                return loadDocumentEntity;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occurred while loading result page", ex);
            }
        }

        public LoadDocumentEntity LoadDocumentPage(PostedDataEntity postedData)
        {
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();

            string password = "";
            try
            {
                // get/set parameters
                string documentGuid = postedData.path;
                int pageNumber = postedData.page;
                password = (String.IsNullOrEmpty(postedData.password)) ? null : postedData.password;
                Comparer comparer = new Comparer();
                List<PageImage> resultImages = comparer.ConvertToImages(documentGuid);


                PageDescriptionEntity loadedPage = new PageDescriptionEntity();
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    resultImages[pageNumber - 1].PageStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                loadedPage.SetData(Convert.ToBase64String(bytes));
                loadedPage.number = pageNumber;
                loadDocumentEntity.SetPages(loadedPage);

            }
            catch (System.Exception ex)
            {
                // set exception message
                throw new Exception("Exception occurred while loading result page", ex);
            }
            return loadDocumentEntity;
        }

        public LoadDocumentEntity LoadDocumentInfo(PostedDataEntity postedData)
        {
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();

            string password = "";
            try
            {
                // get/set parameters
                string documentGuid = postedData.path;                
                password = (String.IsNullOrEmpty(postedData.password)) ? null : postedData.password;
                Comparer comparer = new Comparer();
                List<PageImage> resultImages = comparer.ConvertToImages(documentGuid);

                foreach (PageImage page in resultImages)
                {
                    PageDescriptionEntity loadedPage = new PageDescriptionEntity();                   
                    loadDocumentEntity.SetPages(loadedPage);
                }
                return loadDocumentEntity;                
            }
            catch (System.Exception ex)
            {
                // set exception message
                throw new Exception("Exception occurred while loading document info", ex);
            }           
        }

        private CompareResultResponse MultiCompareFiles(CompareRequest compareRequest)
        {
            ICompareResult compareResult;

            // create new comparer
            MultiComparer multiComparer = new MultiComparer();
            // create setting for comparing
            ComparisonSettings settings = new ComparisonSettings();

            // transform lists of files and passwords
            List<Stream> files = new List<Stream>();
            List<string> passwords = new List<string>();
            Stream firstFile = new FileStream(compareRequest.guids[0].GetGuid(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            string extension = Path.GetExtension(compareRequest.guids[0].GetGuid());
            string firstPassword = compareRequest.guids[0].GetPassword();
            for (int i = 1; i < compareRequest.guids.Count; i++)
            {
                Stream st = new FileStream(compareRequest.guids[i].GetGuid(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                files.Add(st);
                passwords.Add(compareRequest.guids[i].GetPassword());

            }
            // compare two documents
            compareResult = multiComparer.Compare(firstFile, firstPassword, files, passwords, settings);
            firstFile.Dispose();
            firstFile.Close();
            foreach (Stream st in files)
            {
                st.Dispose();
                st.Close();
            }
            if (compareResult == null)
            {
                throw new Exception("Something went wrong. We've got null result.");
            }

            CompareResultResponse compareResultResponse = GetCompareResultResponse(compareResult, extension);
            compareResultResponse.SetExtension(extension);
            return compareResultResponse;
        }

        private CompareResultResponse CompareTwoDocuments(CompareRequest compareRequest)
        {
            string firstPath = compareRequest.guids[0].GetGuid();

            ICompareResult compareResult;
            // create new comparer
            Comparer comparer = new Comparer();
            // create setting for comparing
            ComparisonSettings settings = new ComparisonSettings();
            settings.StyleChangeDetection = true;
            // compare two documents
            compareResult = comparer.Compare(firstPath,
                compareRequest.guids[0].GetPassword(),
                compareRequest.guids[1].GetGuid(),
                compareRequest.guids[1].GetPassword(),
                settings);

            if (compareResult == null)
            {
                throw new Exception("Something went wrong. We've got null result.");
            }

            string extension = Path.GetExtension(firstPath);


            CompareResultResponse compareResultResponse = GetCompareResultResponse(compareResult, extension);
            compareResultResponse.SetExtension(extension);
            return compareResultResponse;
        }

        private CompareResultResponse GetCompareResultResponse(ICompareResult compareResult, string ext)
        {
            CompareResultResponse compareResultResponse = new CompareResultResponse();

            // list of changes
            ChangeInfo[] changes = compareResult.GetChanges();
            compareResultResponse.SetChanges(changes);

            string guid = System.Guid.NewGuid().ToString();
            compareResultResponse.SetGuid(guid);
            //save all results in file
            string resultGuid = SaveFile(compareResultResponse.GetGuid(), compareResult.GetStream(), ext);
            List<PageDescriptionEntity> pages = LoadDocumentPages(resultGuid).GetPages();
            List<PageDescriptionEntity> pageImages = new List<PageDescriptionEntity>();
            foreach (PageDescriptionEntity page in pages)
            {
                PageDescriptionEntity pageData = new PageDescriptionEntity();
                pageData.SetData(page.GetData());
                pageImages.Add(pageData);
            }
            compareResultResponse.SetPages(pageImages);
            compareResultResponse.SetGuid(resultGuid);
            return compareResultResponse;
        }

        private string SaveFile(string guid, Stream inputStream, string ext)
        {
            string fileName = Path.Combine(globalConfiguration.Comparison.GetResultDirectory(), guid + ext);
            try
            {
                using (var fileStream = File.Create(fileName))
                {
                    inputStream.Seek(0, SeekOrigin.Begin);
                    inputStream.CopyTo(fileStream);
                    inputStream.Close();
                }
            }
            catch (IOException)
            {
                throw new Exception("Exception occurred while write result images files.");
            }
            return fileName;
        }

        /// <summary>
        /// Fix file extensions for some formats
        /// </summary>
        /// <param name="ext">string</param>
        /// <returns></returns>
        private string GetRightExt(string ext)
        {
            switch (ext)
            {
                case ".doc":
                    return DOC;
                case ".docx":
                    return DOCX;
                case ".xls":
                    return XLS;
                case ".xlsx":
                    return XLSX;
                case ".ppt":
                    return PPT;
                case ".pptx":
                    return PPTX;
                case ".pdf":
                    return PDF;
                case ".txt":
                    return TXT;
                case ".html":
                    return HTML;
                case ".htm":
                    return HTM;
                default:
                    return ext;
            }
        }

        /// <summary>
        /// Check support formats for comparing
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>string</returns>
        private bool CheckSupportedFiles(string extension)
        {
            switch (extension)
            {
                case ".doc":
                case ".docx":
                case ".xls":
                case ".xlsx":
                case ".ppt":
                case ".pptx":
                case ".pdf":
                case ".txt":
                case ".html":
                case ".htm":
                    return true;
                default:
                    return false;
            }
        }
    }
}
