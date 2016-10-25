using System.Collections.Generic;

using Orchard.Autoroute.Settings;
using Orchard.ContentManagement;

namespace Orchard.Autoroute.Services
{
    public interface IAliasPatternProvider : IDependency
    {
        string GetPatternFor(IContent content);

        RoutePattern GetDefaultPattern(string contentType, string culture);

        void CreatePattern(string contentType, string name, string pattern, string description, bool makeDefault);

        IEnumerable<RoutePattern> GetPatterns(string contentType);
    }
}
