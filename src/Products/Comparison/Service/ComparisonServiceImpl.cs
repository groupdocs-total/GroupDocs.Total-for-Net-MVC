﻿using System;
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
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        //public string CalculateResultFileName(string documentGuid, string index, string ext)
        //{
        //    // configure file name for results
        //    string directory = globalConfiguration.Comparison.GetResultDirectory();
        //    string resultDirectory = String.IsNullOrEmpty(directory) ? globalConfiguration.Comparison.GetFilesDirectory() : directory;
        //    if (!Directory.Exists(resultDirectory))
        //    {
        //        Directory.CreateDirectory(resultDirectory);
        //    }
        //    string extension = (ext != null) ? GetRightExt(ext) : "";
        //    // for images of pages specify index, for all result pages file specify "all" prefix
        //    string idx = (String.IsNullOrEmpty(index)) ? "all" : index;
        //    string suffix = idx + extension;
        //    return string.Format("{0}{1}{2}-{3}-{4}", resultDirectory, Path.DirectorySeparatorChar, COMPARE_RESULT, documentGuid, suffix);
        //}       

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
            //string saveTemp = null;
            //if (Path.GetExtension(firstPath).Equals(".html") || Path.GetExtension(firstPath).Equals(".htm"))
            //{
            //    saveTemp = Path.Combine(globalConfiguration.Comparison.GetResultDirectory(), "temp.html");
            //}
            // convert results
            //save all results in file
            string extension = Path.GetExtension(firstPath);
            CompareResultResponse compareResultResponse = GetCompareResultResponse(compareResult, extension);

           
           // SaveFile(compareResultResponse.GetGuid(), null, compareResult.GetStream(), extension);
            //if (!String.IsNullOrEmpty(saveTemp))
            //{
            //    File.Delete(saveTemp);
            //}
            compareResultResponse.SetExtension(extension);

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

                foreach (PageImage page in resultImages) {
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

        public CompareResultResponse MultiCompareFiles(List<Stream> files, List<string> passwords, string ext)
        {
            ICompareResult compareResult;


            // create new comparer
            MultiComparer multiComparer = new MultiComparer();
            // create setting for comparing
            ComparisonSettings settings = new ComparisonSettings();

            // transform lists of files and passwords
            List<Stream> newFiles = new List<Stream>();
            List<string> newPasswords = new List<string>();
            for (int i = 1; i < files.Count; i++)
            {
                newFiles.Add(files[i]);
                newPasswords.Add(passwords[i]);
            }

            // compare two documents
            compareResult = multiComparer.Compare(files[0], passwords[0], newFiles, newPasswords, settings);


            if (compareResult == null)
            {
                throw new Exception("Something went wrong. We've got null result.");
            }
            string saveTemp = null;
            if(ext.Equals("html") || ext.Equals("htm"))
            {
                saveTemp = Path.Combine(globalConfiguration.Comparison.GetResultDirectory(), "temp.html");
            }
            // convert results
            CompareResultResponse compareResultResponse = GetCompareResultResponse(compareResult, saveTemp);

           
            if (!String.IsNullOrEmpty(saveTemp))
            {
                File.Delete(saveTemp);
            }
            compareResultResponse.SetExtension(ext);

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
            List<string> pageImages = new List<string>();
            foreach (PageDescriptionEntity page in pages)
            {
                pageImages.Add(page.GetData());
            }
            compareResultResponse.SetPages(pageImages);
            compareResultResponse.SetGuid(resultGuid);
            return compareResultResponse;
        }

        //private List<string> SaveImages(Stream[] images, string guid)
        //{
        //    List<string> paths = new List<string>(images.Length);
        //    for (int i = 0; i < images.Length; i++)
        //    {
        //        paths.Add(SaveFile(guid, i.ToString(), images[i], JPG));
        //    }
        //    return paths;
        //}

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
