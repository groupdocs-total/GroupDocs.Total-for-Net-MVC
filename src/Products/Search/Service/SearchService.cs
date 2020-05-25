using GroupDocs.Total.MVC.Products.Search.Entity.Web.Request;
using GroupDocs.Search;
using GroupDocs.Search.Results;
using GroupDocs.Total.MVC.Products.Search.Entity.Web.Response;
using System.Collections.Generic;
using GroupDocs.Search.Options;
using GroupDocs.Search.Highlighters;
using GroupDocs.Search.Common;
using System.IO;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Config;
using System;

namespace GroupDocs.Total.MVC.Products.Search.Service
{
    public static class SearchService
    {
        private static Index index;
        internal static Dictionary<string, DocumentStatus> filesIndexStatuses = new Dictionary<string, DocumentStatus>();

        public static SummarySearchResult Search(SearchPostedData searchRequest, Common.Config.GlobalConfiguration globalConfiguration)
        {
            if (index == null) return new SummarySearchResult();

            SearchResult result = index.Search(searchRequest.GetQuery());

            SummarySearchResult summaryResult = new SummarySearchResult();
            List<SearchDocumentResult> foundFiles = new List<SearchDocumentResult>();

            HighlightOptions options = new HighlightOptions
            {
                TermsBefore = 5,
                TermsAfter = 5,
                TermsTotal = 10
            };

            for (int i = 0; i < result.DocumentCount; i++)
            {
                SearchDocumentResult searchDocumentResult = new SearchDocumentResult();

                FoundDocument document = result.GetFoundDocument(i);
                HtmlFragmentHighlighter highlighter = new HtmlFragmentHighlighter();
                index.Highlight(document, highlighter, options);
                FragmentContainer[] fragmentContainers = highlighter.GetResult();

                List<string> foundPhrases = new List<string>();
                for (int j = 0; j < fragmentContainers.Length; j++)
                {
                    FragmentContainer container = fragmentContainers[j];
                    string[] fragments = container.GetFragments();
                    if (fragments.Length > 0)
                    {
                        for (int k = 0; k < fragments.Length; k++)
                        {
                            foundPhrases.Add(fragments[k]);
                        }
                    }
                }

                searchDocumentResult.SetFoundFields(document.FoundFields);
                searchDocumentResult.SetGuid(document.DocumentInfo.FilePath);
                searchDocumentResult.SetName(Path.GetFileName(document.DocumentInfo.FilePath));
                searchDocumentResult.SetSize(new FileInfo(document.DocumentInfo.FilePath).Length);
                searchDocumentResult.SetOccurrences(document.OccurrenceCount);
                searchDocumentResult.SetFoundPhrases(foundPhrases.ToArray());

                foundFiles.Add(searchDocumentResult);
            }

            summaryResult.SetFoundFiles(foundFiles.ToArray());
            summaryResult.SetTotalOccurences(result.OccurrenceCount);
            summaryResult.SetTotalFiles(result.DocumentCount);
            string searchDurationString = result.SearchDuration.ToString(@"ss\.ff");
            summaryResult.SetSearchDuration(searchDurationString.Equals("00.00") ? "< 1" : searchDurationString);
            summaryResult.SetIndexedFiles(Directory.GetFiles(globalConfiguration.GetSearchConfiguration().GetFilesDirectory(), "*", SearchOption.TopDirectoryOnly).Length);

            return summaryResult;
        }

        internal static void InitIndex(GlobalConfiguration globalConfiguration)
        {
            if (index == null)
            {
                string indexedFilesDirectory = globalConfiguration.GetSearchConfiguration().GetIndexedFilesDirectory();
                index = new Index();

                index.Events.OperationProgressChanged += (sender, args) =>
                {
                    if (!filesIndexStatuses.ContainsKey(args.LastDocumentPath))
                    {
                        filesIndexStatuses.Add(args.LastDocumentPath, args.LastDocumentStatus);
                    }
                };

                index.Add(indexedFilesDirectory);
            }
        }

        internal static void AddFilesToIndex(PostedDataEntity[] postedData, GlobalConfiguration globalConfiguration)
        {
            string indexedFilesDirectory = globalConfiguration.GetSearchConfiguration().GetIndexedFilesDirectory();

            foreach (var entity in postedData) 
            {
                string fileName = Path.GetFileName(entity.guid);
                string destFileName = Path.Combine(indexedFilesDirectory, fileName);
                
                if (!File.Exists(destFileName))
                {
                    File.Copy(entity.guid, destFileName);
                }
            }

            index.Update(GetUpdateOptions());
        }

        internal static void RemoveFileFromIndex(string guid)
        {
            if (File.Exists(guid))
            {
                File.Delete(guid);
            }

            index.Update(GetUpdateOptions());
        }

        private static UpdateOptions GetUpdateOptions()
        {
            return new UpdateOptions
            {
                Threads = 2
            };
        }
    }
}