using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Aliases
{
    public interface IAliasRule : IDependency
    {
        bool Match(IContent contentItem);

        void Apply(IContent contentItem, AliasBuildContext context);

        string Rule { get; }

        int Priority { get; }

    }
}