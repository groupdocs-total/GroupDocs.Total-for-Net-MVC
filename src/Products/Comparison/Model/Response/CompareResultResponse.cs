using GroupDocs.Comparison.Common.Changes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Comparison.Model.Response
{
    public class CompareResultResponse
    {
        /// <summary>
        /// List of changies
        /// </summary>
        [JsonProperty]
        private ChangeInfo[] changes;

        /// <summary>
        /// List of images of pages with marked changes
        /// </summary>
        [JsonProperty]
        private List<PageData> pages;

        /// <summary>
        /// Unique key of results
        /// </summary>
        [JsonProperty]
        private string guid;

        /// <summary>
        /// Extension of compared files, for saving total results
        /// </summary>
        [JsonProperty]
        private string extension;

        public void SetChanges(ChangeInfo[] changes)
        {
            this.changes = changes;
        }

        public ChangeInfo[] GetChanges()
        {
            return changes;
        }

        public void SetPages(List<PageData> pages)
        {
            this.pages = pages;
        }

        public void AddPage(PageData page)
        {
            this.pages.Add(page);
        }

        public List<PageData> GetPages()
        {
            return pages;
        }

        public void SetGuid(string guid)
        {
            this.guid = guid;
        }

        public string GetGuid()
        {
            return guid;
        }

        public void SetExtension(string extension)
        {
            this.extension = extension;
        }

        public string GetExtension()
        {
            return extension;
        }
    }
}