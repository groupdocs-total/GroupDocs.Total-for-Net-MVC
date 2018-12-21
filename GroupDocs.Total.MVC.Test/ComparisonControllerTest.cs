﻿using GroupDocs.Total.MVC.Controllers;
using NUnit.Framework;
using System.Web.Routing;
using MvcContrib.TestHelper;
using Huygens;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace GroupDocs.Total.MVC.Test
{
    [TestFixture]
    public class ComparisonControllerTest
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
        public void ViewStatusTest()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "/../../../src";            
            using (var server = new DirectServer(path))
            {
                var request = new SerialisableRequest
                {
                    Method = "GET",
                    RequestUri = "/comparison",
                    Content = null
                };

                var result = server.DirectCall(request);
                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public void ViewMapControllerTest()
        {
            "~/comparison".Route().ShouldMapTo<ComparisonController>(x => x.Index());
        }

        //[Test]
        //public void FileTreeStatusCodeTest()
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "/../../../src";
        //    using (var server = new DirectServer(path))
        //    {
        //        var request = new SerialisableRequest
        //        {
        //            Method = "POST",
        //            RequestUri = "/comparison/loadfiletree",
        //            Content = null,
        //            Headers = new Dictionary<string, string>{
        //                { "Content-Type", "application/json"}
        //            }
        //        };

        //        var result = server.DirectCall(request);
        //        Assert.That(result.StatusCode, Is.EqualTo(200));
        //    }
        //}

        //[Test]
        //public void FileTreeDataTest()
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "/../../../src";
        //    using (var server = new DirectServer(path))
        //    {
        //        var request = new SerialisableRequest
        //        {
        //            Method = "POST",
        //            RequestUri = "/comparison/loadfiletree",
        //            Content = null,
        //            Headers = new Dictionary<string, string>{
        //                { "Content-Type", "application/json"}
        //            }
        //        };

        //        var result = server.DirectCall(request);
        //        var resultString = Encoding.UTF8.GetString(result.Content);
        //        dynamic data = JsonConvert.DeserializeObject(resultString);
        //        Assert.IsTrue(data.Count > 0);
        //    }
        //}
    }
}
