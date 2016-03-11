using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Presenters
{
    public class CSRFPresenter : ICSRFPresenter
    {
        public string TokenName { get; } = "__RequestVerificationToken";

        public string Code { get; private set; }

        public CSRFPresenter(HttpRequestBase request, HttpResponseBase response)
        {
            var cookieStored = request.Cookies.Get(TokenName);

            if(cookieStored != null)
            {
                Code = cookieStored.Value;
            }
            else
            {
                Code = Guid.NewGuid().ToString();
            }

            response.Cookies.Add(new HttpCookie(TokenName, Code));
        }
    }
}