using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    /// <summary>
    /// Server configuration
    /// </summary>
    public class ServerConfiguration : ConfigurationSection
    {
        public int HttpPort = 8080;
        public string HostAddress = "localhost";
        private NameValueCollection serverConfiguration = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("serverConfiguration");

        /// <summary>
        /// Get server configuration section of the web.config
        /// </summary>
        public ServerConfiguration() {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("server");
            HttpPort = (configuration != null && !String.IsNullOrEmpty(configuration["connector"]["port"].ToString())) ?
                Convert.ToInt32(configuration["connector"]["port"]) : 
                Convert.ToInt32(serverConfiguration["httpPort"]);
            HostAddress = (!String.IsNullOrEmpty(serverConfiguration["hostAddress"])) ?
                serverConfiguration["hostAddress"] : 
                HostAddress;
        }
    }
}