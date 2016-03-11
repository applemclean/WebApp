using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Presenters;

namespace WebApp.Controllers
{
    public class MainController : Controller
    {
        private readonly Lazy<ICSRFPresenter> _csrfPresenter;

        public MainController()
        {
            _csrfPresenter = new Lazy<ICSRFPresenter>(() => new CSRFPresenter(Request, Response));
        }

        public MainController(ICSRFPresenter csrfPresenter)
        {
            if(csrfPresenter == null)
            {
                throw new ArgumentNullException(nameof(csrfPresenter));
            }

            _csrfPresenter = new Lazy<ICSRFPresenter>(() => csrfPresenter);
        }

        public ActionResult Index()
        {
            return View(_csrfPresenter.Value);
        }
    }
}