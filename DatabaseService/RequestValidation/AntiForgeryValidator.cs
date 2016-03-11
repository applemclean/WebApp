using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DatabaseService.RequestValidation
{
    public class AntiForgeryValidator : IAntiForgeryValidator
    {
        public const string TokenName = "__RequestVerificationToken";

        /// <summary>
        /// Validate CSRF tokens (cookie and header __RequestVerificationToken).
        /// </summary>
        /// <param name="request"><c>HttpRequestMessageProperty</c></param>
        /// <returns>Validation success</returns>
        public bool Validate(HttpRequestMessageProperty request)
        {
            var cookies = request.Headers[HttpRequestHeader.Cookie];

            if (cookies == default(string))
            {
                return false;
            }

            var cookieCSRFKeyValue = cookies.Split(';')
                                    .Select(x => x.Split('='))
                                    .FirstOrDefault(x => x[0].Trim() == TokenName);

            if(cookieCSRFKeyValue == null)
            {
                return false;
            }

            var cookieCSRF = cookieCSRFKeyValue[1];

            var csrfSent = request.Headers.Get(TokenName);

            if (csrfSent == default(string))
            {
                return false;
            }

            // As soon as Antiforgery.Validate isn't available outside IIS
            return cookieCSRF == csrfSent;
        }
    }
}