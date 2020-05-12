using GroupDocs.Total.MVC.Products.Search.Entity.Web.Request;
using GroupDocs.Search;
using GroupDocs.Search.Results;
using GroupDocs.Total.MVC.Products.Search.Entity.Web.Response;
using System.Collections.Generic;
using GroupDocs.Search.Options;
using GroupDocs.Search.Highlighters;
using GroupDocs.Search.Common;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Search.Service
{
    public static class SearchService
    {
        public static SummarySearchResult Search(Index index, SearchPostedData searchRequest, Common.Config.GlobalConfiguration globalConfiguration)
        {
            SearchResult result = index.Search(searchRequest.GetQuery());

            SummarySearchResult summaryResult = new SummarySearchResult();
            List<SearchDocumentResult> foundFiles = new List<SearchDocumentResult>();

            // Assigning highlight options
            HighlightOptions options = new HighlightOptions();
            options.TermsBefore = 5;
            options.TermsAfter = 5;
            options.TermsTotal = 15;

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
            summaryResult.SetSearchDuration(result.SearchDuration.ToString(@"mm\:ss\.f"));

            return summaryResult;
        }
    }
}