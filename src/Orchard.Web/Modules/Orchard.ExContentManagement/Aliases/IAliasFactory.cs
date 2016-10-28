using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Aliases
{
    public interface IAliasFactory : IDependency
    {
        AliasBuildResult BuidFor(IContent contentItem);
    }
}