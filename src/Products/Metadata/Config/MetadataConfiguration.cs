using System;
using System.Collections.Specialized;
using System.Configuration;

namespace GroupDocs.Total.MVC.Products.Metadata.Config
{
    /// <summary>
    /// MetadataConfiguration
    /// </summary>
    public class MetadataConfiguration : ConfigurationSection
    {
        public string FilesDirectory { get; set; }
        public bool isHidden { get; set; }
        public bool isCustom { get; set; }
        public bool isMultimedia { get; set; }
        private NameValueCollection signatureConfiguration = (NameValueCollection)ConfigurationManager.GetSection("metadataConfiguration");

        /// <summary>
        /// Get metadata configuration section from the Web.config
        /// </summary>
        public MetadataConfiguration()
        {
            FilesDirectory = signatureConfiguration["filesDirectory"];
            isHidden = Convert.ToBoolean(signatureConfiguration["isHidden"]);
            isCustom = Convert.ToBoolean(signatureConfiguration["isCustom"]);
            isMultimedia = Convert.ToBoolean(signatureConfiguration["isMultimedia"]);           
        }
    }
}