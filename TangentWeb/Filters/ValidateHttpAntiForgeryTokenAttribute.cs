using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace TangentWeb.Filters
{
  // CSC prevention notes found here:
  // http://www.asp.net/web-api/overview/security/preventing-cross-site-request-forgery-%28csrf%29-attacks
  public class ValidateHttpAntiForgeryTokenAttribute : AuthorizationFilterAttribute
  {
    public override void OnAuthorization(HttpActionContext actionContext)
    {
      HttpRequestMessage request = actionContext.ControllerContext.Request;
      try
      {
        // Provided GET Requests don't modify data we don't need to validate a RequestVerificationToken 
        // https://www.owasp.org/index.php/Cross-Site_Request_Forgery_(CSRF)_Prevention_Cheat_Sheet
        if (request.Method != HttpMethod.Get)
        {
          if (IsAjaxRequest(request))
          {
            ValidateRequestHeader(request);
          }
          else
          {
            AntiForgery.Validate();
          }
        }
      }
      catch (HttpAntiForgeryException e)
      {
        actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, e);
      }
    }

    private bool IsAjaxRequest(HttpRequestMessage request)
    {
      IEnumerable<string> xRequestedWithHeaders;
      if (request.Headers.TryGetValues("X-Requested-With", out xRequestedWithHeaders))
      {
        string headerValue = xRequestedWithHeaders.FirstOrDefault();
        if (!String.IsNullOrEmpty(headerValue))
        {
          return String.Equals(headerValue, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        }
      }
      return false;
    }

    private void ValidateRequestHeader(HttpRequestMessage request)
    {
      string cookieToken = String.Empty;
      string formToken = String.Empty;
      IEnumerable<string> tokenHeaders;
      if (request.Headers.TryGetValues("RequestVerificationToken", out tokenHeaders))
      {
        string tokenValue = tokenHeaders.FirstOrDefault();
        if (!String.IsNullOrEmpty(tokenValue))
        {
          string[] tokens = tokenValue.Split(':');
          if (tokens.Length == 2)
          {
            cookieToken = tokens[0].Trim();
            formToken = tokens[1].Trim();
          }
        }
      }
      AntiForgery.Validate(cookieToken, formToken);
    }
  }
}