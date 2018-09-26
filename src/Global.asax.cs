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
            string annotationAssemblyName = "GroupDocs.Annotation.dll";
            // set GroupDocs.Viewer license
            DomainGenerator viewerDomainGenerator = new DomainGenerator(viewerAssemblyName, "GroupDocs.Viewer.License");
            viewerDomainGenerator.SetViewerLicense(viewerDomainGenerator.CurrentType);
            // set GroupDocs.Signature license
            DomainGenerator signatureDomainGenerator = new DomainGenerator(signatureAssemblyName, "GroupDocs.Signature.License");
            signatureDomainGenerator.SetSignatureLicense(signatureDomainGenerator.CurrentType);
            // set GroupDocs.Annotation license
            DomainGenerator annotationDomainGenerator = new DomainGenerator(annotationAssemblyName, "GroupDocs.Annotation.Common.License.License");
            annotationDomainGenerator.SetAnnotationLicense(annotationDomainGenerator.CurrentType);
           
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }       
    }     
}