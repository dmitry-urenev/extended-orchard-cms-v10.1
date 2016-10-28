using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement
{
    public interface ILocalizableContentService : IDependency
    {
        string ResolveLocalizedUrl(string relativeUrl, string culture = null);
    }
}