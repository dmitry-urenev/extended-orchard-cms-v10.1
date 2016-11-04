using System;
using System.Collections.Generic;
using Orchard;
using Orchard.SEO.Models;
using Orchard.Environment.Extensions;

namespace Orchard.SEO.Services
{
    public interface ISitemapService : IDependency
    {
        SitemapFileRecord Get();

        bool Save(string text);
    }
}