using Orchard.StaticPages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.StaticPages.Services
{
    public interface IStaticPageService : IDependency
    {
        void UpdateRoute(StaticPagePart part, string staticUrl);
        void UpdateRoute(StaticPagePart part, IDictionary<string, string> routeValues);

        void Route(StaticPagePart part, string path);

        void RemoveAliases(StaticPagePart part);
    }
}