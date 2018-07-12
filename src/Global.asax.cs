using GroupDocs.Total.MVC.AppDomainGenerator;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace GroupDocs.Total.MVC
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Fix required to use several GroupDocs products in one project.
            // Set GroupDocs products assemblies names
            string viewerAssemblyName = "GroupDocs.Viewer.dll";
            string signatureAssemblyName = "GroupDocs.Signature.dll";
            // Create AppDomain for the GroupDocs.Viewer
            DomainGenerator viewerDomainGenerator = new DomainGenerator();
            // Get assembly path
            string assemblyPath = viewerDomainGenerator.GetAssemblyPath(viewerAssemblyName);
            // Initiate GroupDocs license class
            Type type = viewerDomainGenerator.CreateDomain("ViewerDomain", assemblyPath, "GroupDocs.Viewer.License");
            // Run SetLicense mathod from the initiated class
            viewerDomainGenerator.SetViewerLicense(type);
            // Create AppDomain for the GroupDocs.Signature
            DomainGenerator signatureDomainGenerator = new DomainGenerator();
            // Get assembly path
            string signatureAssemblyPath = signatureDomainGenerator.GetAssemblyPath(signatureAssemblyName);
            // Initiate Licenseclass
            Type signatureType = signatureDomainGenerator.CreateDomain("SignatureDomain", signatureAssemblyPath, "GroupDocs.Signature.License");
            // Run SetLicense mathod for GroupDocs.Signature
            signatureDomainGenerator.SetSignatureLicense(signatureType);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }       
    }     
}
