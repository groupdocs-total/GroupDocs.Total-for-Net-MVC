﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Common.Entity.Web
{
    /// <summary>
    /// DTO-class, represents document properties and its pages collection.
    /// </summary>
    public class LoadDocumentEntity
    {
        /// <summary>
        /// Document absolute path.
        /// </summary>
        [JsonProperty]
        private string guid;

        /// <summary>
        /// Collection of the document pages with their data.
        /// </summary>
        [JsonProperty]
        private List<PageDescriptionEntity> pages = new List<PageDescriptionEntity>();

        /// <summary>
        /// Document print allowed flag.
        /// </summary>
        [JsonProperty]
        private bool printAllowed = true;

        public void SetPrintAllowed(bool allowed)
        {
            this.printAllowed = allowed;
        }

        public bool GetPrintAllowed()
        {
            return this.printAllowed;
        }

        public void SetGuid(string guid)
        {
            this.guid = guid;
        }

        public string GetGuid()
        {
            return this.guid;
        }

        public void SetPages(PageDescriptionEntity page)
        {
            this.pages.Add(page);
        }

        public List<PageDescriptionEntity> GetPages()
        {
            return this.pages;
        }
    }
}