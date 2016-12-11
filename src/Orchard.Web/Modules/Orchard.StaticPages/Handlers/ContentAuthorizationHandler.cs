using Orchard.Mvc;
using Orchard.Security;
using Orchard.StaticPages.Models;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;
using Orchard.Logging;


namespace Orchard.StaticPages.Handlers
{
    public class ContentAuthorizationHandler : IAuthorizationServiceEventHandler
    {
        private Func<ControllerContext, ActionDescriptor, IEnumerable<Filter>> _getFiltersThunk = FilterProviders.Providers.GetFilters;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ContentAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IWorkContextAccessor workContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _workContextAccessor = workContextAccessor;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }        

        public bool IsAuthorized(StaticPagePart part)
        {
            var rv = part.RouteValues;
            string area = part.Route.Action.Area;
            string controller = part.Route.Action.Controller;
            string action = part.Route.Action.Action;

            var aContext = GetAuthorizationContext(area, controller, action);

            return aContext != null && aContext.Result == null;
        }       

        private AuthorizationContext GetAuthorizationContext(string areaName, string controllerName, string actionName)
        {
            AuthorizationContext authorizationContext = null;

            try
            {
                var httpContext = _httpContextAccessor.Current();
                var requestContext = new RequestContext()
                {
                    HttpContext = httpContext,
                    RouteData = new RouteData()
                };
                requestContext.RouteData.DataTokens["area"] = areaName;
                requestContext.RouteData.DataTokens["IWorkContextAccessor"] = _workContextAccessor;

                var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
                var targetController = (ControllerBase)controllerFactory.CreateController(requestContext, controllerName);

                var controllerDescriptor = new ReflectedControllerDescriptor(targetController.GetType());

                var controllerContext = new ControllerContext(httpContext, requestContext.RouteData, targetController);

                var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

                FilterInfo filterInfo = GetFilters(controllerContext, actionDescriptor);

                authorizationContext = InvokeAuthorizationFilters(controllerContext, actionDescriptor, filterInfo.AuthorizationFilters);
            }
            catch { }

            return authorizationContext;
        }

        protected virtual FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return new FilterInfo(_getFiltersThunk(controllerContext, actionDescriptor));
        }

        private AuthorizationContext InvokeAuthorizationFilters(
           ControllerContext controllerContext,
           ActionDescriptor actionDescriptor,
           IList<IAuthorizationFilter> filters)
        {
            AuthorizationContext context = new AuthorizationContext(controllerContext, actionDescriptor);
            foreach (IAuthorizationFilter filter in filters)
            {
                filter.OnAuthorization(context);
                // short-circuit evaluation when an error occurs
                if (context.Result != null)
                {
                    break;
                }
            }

            return context;
        }

        public void Checking(CheckAccessContext context)
        {
        }

        public void Adjust(CheckAccessContext context)
        {
        }

        public void Complete(CheckAccessContext context)
        {         
            if (context.Content == null)
                return;

            var part = context.Content.As<StaticPagePart>();
            if (part == null || part.Route == null || part.Route.Action == null)
                return;

            if (!String.IsNullOrEmpty(_workContextAccessor.GetContext().CurrentSite.SuperUser)
             && context.User != null
             && String.Equals(context.User.UserName, _workContextAccessor.GetContext().CurrentSite.SuperUser, StringComparison.Ordinal))
            {
                context.Granted = true;
                return;
            }

            if (!IsAuthorized(part))
            {
                context.Granted = false;
                context.Adjusted = true;
            }
        }
    }
}