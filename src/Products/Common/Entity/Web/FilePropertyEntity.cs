using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupDocs.Metadata.Common;

namespace GroupDocs.Total.MVC.Products.Common.Entity.Web
{
    /// <summary>
    /// File property entity
    /// </summary>
    public class FilePropertyEntity
    {
        public string name { get; set; }
        public dynamic value { get; set; }
        public FilePropertyCategory category { get; set; }
        public bool original { get; set; }
        public MetadataPropertyType type { get; set; }
    }

    public enum FilePropertyCategory
    {
        BuildIn,
        Default
    }
}