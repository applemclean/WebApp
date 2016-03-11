using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp;
using WebApp.Controllers;
using WebApp.Presenters;

namespace WebApp.Tests.Controllers
{
    [TestClass]
    public class MainControllerTest
    {
        private class FakeCSRFPresenter : ICSRFPresenter
        {
            public string Code
            {
                get
                {
                    return "code";
                }
            }

            public string TokenName
            {
                get
                {
                    return "token name";
                }
            }
        }

        [TestMethod]
        public void Index()
        {
            var controller = new MainController(new FakeCSRFPresenter());
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
