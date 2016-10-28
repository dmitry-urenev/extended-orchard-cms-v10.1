using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;

namespace Orchard.Autoroute.Services
{
    public static class RouteUtils
    {
        public static RouteData GetRouteDataByUrl(string url)
        {
            return RouteTable.Routes.GetRouteData(new RewritedHttpContextBase(url));
        }
        public static string GetVirtualPath(string urlTemplate, RouteValueDictionary routeValues)
        {
            foreach (string key in routeValues.Keys)
            {
                var val = routeValues[key];
                urlTemplate = urlTemplate
                    .Replace("{*" + key + "}", "{" + key + "}")
                    .Replace("{" + key + "}", val.ToString());
            }
            return urlTemplate;
        }


        private class RewritedHttpContextBase : HttpContextBase
        {
            private readonly HttpRequestBase mockHttpRequestBase;

            public RewritedHttpContextBase(string appRelativeUrl)
            {
                this.mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
            }


            public override HttpRequestBase Request
            {
                get
                {
                    return mockHttpRequestBase;
                }
            }

            private class MockHttpRequestBase : HttpRequestBase
            {
                private readonly string appRelativeUrl;

                public MockHttpRequestBase(string appRelativeUrl)
                {
                    this.appRelativeUrl = appRelativeUrl;
                }

                public override string AppRelativeCurrentExecutionFilePath
                {
                    get { return appRelativeUrl; }
                }

                public override string PathInfo
                {
                    get { return ""; }
                }

                public override NameValueCollection Headers
                {
                    get
                    {
                        return new NameValueCollection();
                    }
                }
            }
        }
    }
}