using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    /// <summary>
    /// Application configuration
    /// </summary>
    public class ApplicationConfiguration : ConfigurationSection
    {
        public string LicensePath { get; set; }
        private NameValueCollection applicationConfiguration = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("applicationConfiguration");

        /// <summary>
        /// Get license path from the application configuration section of the web.config
        /// </summary>
        public ApplicationConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("application");
            LicensePath = (configuration != null && !String.IsNullOrEmpty(configuration["licensePath"].ToString())) ? configuration["licensePath"] : applicationConfiguration["licensePath"];
            if (String.IsNullOrEmpty(LicensePath)) {
                LicensePath = "Licenses/GroupDocs.Total.NET.lic";
            }
            if (!IsFullPath(LicensePath))
            {
                LicensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LicensePath);
                if (!Directory.Exists(Path.GetDirectoryName(LicensePath)))
                {                    
                    Directory.CreateDirectory(LicensePath);
                }
            }
            if (!File.Exists(LicensePath))
            {
                LicensePath = "";
            }
        }

        private static bool IsFullPath(string path)
        {
            return !String.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
    }
}