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
            if (IsFullPath(applicationConfiguration["licensePath"]))
            {
                LicensePath = applicationConfiguration["licensePath"];
            }
            else
            {
                string rootFolder = AppDomain.CurrentDomain.BaseDirectory;
                LicensePath = Path.Combine(rootFolder, applicationConfiguration["licensePath"]);
                if (!File.Exists(LicensePath))
                {
                    if (!Directory.Exists(Path.Combine(rootFolder, "licenses")))
                    {
                        Directory.CreateDirectory(Path.Combine(rootFolder, "licenses"));
                    }
                    LicensePath = Path.Combine(rootFolder, "licenses/GroupDocs.Total.NET.lic");
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