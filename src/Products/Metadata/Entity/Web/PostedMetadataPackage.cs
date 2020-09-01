using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupDocs.Total.MVC.Products.Metadata.Entity.Web
{
    public class PostedMetadataPackage
    {
        public string id { get; set; }

        public List<FilePropertyEntity> properties { get; set; }
    }
}