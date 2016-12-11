using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Core.Contents.PageContext
{
    public static class WorkContextExtention
    {
        public static PageContext GetPageContext(this WorkContext workContext)
        {
            var pcontext = workContext.GetState<PageContext>("PageContext");
            return pcontext;
        }
    }
}