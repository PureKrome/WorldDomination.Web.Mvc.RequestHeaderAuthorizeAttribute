using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WorldDomination.Web.Mvc
{
    public class RequestHeaderAuthorizeAttribute : AuthorizeAttribute
    {
        public string Header { get; set; }
        public bool RequireSsl { get; set; }
        public ICustomAuthorization Authorization { get; set; }

        public RequestHeaderAuthorizeAttribute()
        {
            Header = "Authorization";
            RequireSsl = false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (Authenticate(filterContext.HttpContext))
            {
                // NOTE: We must have already set the IPrinciple up, by this point before calling the base.OnAuth.
                base.OnAuthorization(filterContext);
            }
            else
            {
                // Not authorized.
                filterContext.Result = new HttpUnauthorizedResult("The provided token was not authorized.");    
            }
        }

        private bool Authenticate(HttpContextBase context)
        {
            if (RequireSsl && 
                !context.Request.IsSecureConnection && 
                !context.Request.IsLocal)
            {
                return false;
            }

            if (!context.Request.Headers.AllKeys.Contains(Header))
            {
                return false;
            }

            string token = context.Request.Headers[Header];

            IPrincipal principal;
            if (Authorization.TryAuthorize(token, out principal))
            {
                context.User = principal;
                return true;
            }

            return false;
        }
    }
}
