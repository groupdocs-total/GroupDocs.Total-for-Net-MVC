﻿using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    internal class StopWordsReadResponse
    {
        [JsonProperty]
        public string[] StopWords { get; set; }
    }
}
