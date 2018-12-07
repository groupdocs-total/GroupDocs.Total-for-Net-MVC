using GroupDocs.Total.MVC.Controllers;
using NUnit.Framework;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;

namespace GroupDocs.Total.MVC.Test
{
    [TestFixture]
    public class ViewerControllerTest
    {  
        [SetUp]
        public void TestInitialize()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }

        [TearDown]
        public void TearDown()
        {
            RouteTable.Routes.Clear();
        }

        [Test]
        public void ViewNameTest() {
            ViewerController viewer = new ViewerController();

            ViewResult actResult = viewer.Index() as ViewResult;            
            Assert.That(actResult.ViewName, Is.EqualTo("Index"));
        }

        [Test]
        public void ViewStatusTest()
        {
            "~/viewer".Route().ShouldMapTo<ViewerController>(x => x.Index());
        }       
    }
}
