using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Core.Contents.PageContext
{
    public class PageContextStateProvider : IWorkContextStateProvider
    {
        private readonly IPageContextHolder _contextHolder;

        public PageContextStateProvider(IPageContextHolder contextHolder)
        {
            _contextHolder = contextHolder;
        }

        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name == "PageContext")
            {
                return ctx =>
                {
                    return (T)(object)_contextHolder.PageContext;
                };
            }
            return null;
        }
    }

    public interface IPageContextHolder : IDependency
    {
        PageContext PageContext { get; }
    }

    public class DefaultPageContextHolder : IPageContextHolder
    {
        private PageContext _pageContext;

        public PageContext PageContext
        {
            get
            {
                if (_pageContext == null)
                {
                    _pageContext = new PageContext();
                }
                return _pageContext;
            }
        }
    }
}