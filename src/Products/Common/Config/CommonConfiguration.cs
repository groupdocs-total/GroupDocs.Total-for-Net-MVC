using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    /// <summary>
    /// CommonConfiguration
    /// </summary>
    public class CommonConfiguration : ConfigurationSection
    {
        public bool isPageSelector { get; set; }    
        public bool isDownload { get; set; }
        public bool isUpload { get; set; }
        public bool isPrint { get; set; }
        public bool isBrowse { get; set; }
        public bool isRewrite { get; set; }
        private NameValueCollection commonConfiguration = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("commonConfiguration");

        /// <summary>
        /// Constructor
        /// </summary>
        public CommonConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("common");
            isPageSelector = (configuration != null && !String.IsNullOrEmpty(configuration["pageSelector"].ToString())) ?
                Convert.ToBoolean(configuration["pageSelector"]) :
                Convert.ToBoolean(commonConfiguration["isPageSelector"]);               
            isDownload = (configuration != null && !String.IsNullOrEmpty(configuration["download"].ToString())) ?
                Convert.ToBoolean(configuration["download"]) :
                Convert.ToBoolean(commonConfiguration["isDownload"]);
            isUpload = (configuration != null && !String.IsNullOrEmpty(configuration["upload"].ToString())) ?
                Convert.ToBoolean(configuration["upload"]) :
                Convert.ToBoolean(commonConfiguration["isUpload"]);
            isPrint = (configuration != null && !String.IsNullOrEmpty(configuration["print"].ToString())) ?
                Convert.ToBoolean(configuration["print"]) :
                Convert.ToBoolean(commonConfiguration["isPrint"]);
            isBrowse = (configuration != null && !String.IsNullOrEmpty(configuration["browse"].ToString())) ?
                Convert.ToBoolean(configuration["browse"]) : 
                Convert.ToBoolean(commonConfiguration["isBrowse"]);         
            isRewrite = (configuration != null && !String.IsNullOrEmpty(configuration["rewrite"].ToString())) ?
                Convert.ToBoolean(configuration["rewrite"]) : 
                Convert.ToBoolean(commonConfiguration["isRewrite"]);
        }
    }
}