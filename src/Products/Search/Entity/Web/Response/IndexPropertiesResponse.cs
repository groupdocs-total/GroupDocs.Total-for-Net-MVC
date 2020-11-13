﻿using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    internal class IndexProperties
    {
        [JsonProperty]
        public string IndexVersion { get; set; }

        [JsonProperty]
        public string IndexType { get; set; }

        [JsonProperty]
        public bool UseStopWords { get; set; }

        [JsonProperty]
        public bool UseCharacterReplacements { get; set; }

        [JsonProperty]
        public bool AutoDetectEncoding { get; set; }

        [JsonProperty]
        public bool UseRawTextExtraction { get; set; }

        [JsonProperty]
        public string TextStorageCompression { get; set; }
    }
}
