﻿using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    internal class DocumentPasswordsReadResponse
    {
        [JsonProperty]
        public KeyPasswordPair[] Passwords { get; set; }
    }
}
