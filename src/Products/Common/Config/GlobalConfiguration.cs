﻿using GroupDocs.Total.MVC.Products.Signature.Config;
using GroupDocs.Total.MVC.Products.Viewer.Config;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    /// <summary>
    /// Global configuration
    /// </summary>
    public class GlobalConfiguration
    {
        public ServerConfiguration Server;
        public ApplicationConfiguration Application;
        public SignatureConfiguration Signature;
        public ViewerConfiguration Viewer;

        /// <summary>
        /// Get all configurations
        /// </summary>
        public GlobalConfiguration()
        {
            Server = new ServerConfiguration();
            Application = new ApplicationConfiguration();
            Signature = new SignatureConfiguration();
            Viewer = new ViewerConfiguration();
        }
    }
}