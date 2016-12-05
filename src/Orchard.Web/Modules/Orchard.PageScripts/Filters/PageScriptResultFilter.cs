using Orchard.DisplayManagement;
using Orchard.Mvc.Filters;
using Orchard.PageScripts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Orchard.PageScripts.Filters
{
    public class PageScriptResultFilter : FilterProvider, IResultFilter
    {
        private IWorkContextAccessor _workContextAccessor;
        private readonly dynamic _shapeFactory;
        private readonly IPageScriptService _scriptService;

        public PageScriptResultFilter(
            IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory,
            IPageScriptService scriptService)
        {
            _workContextAccessor = workContextAccessor;
            _shapeFactory = shapeFactory;
            _scriptService = scriptService;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) { }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result as ViewResult == null)
            {
                return;
            }

            var sc = _scriptService.GetScripts();

            var layout = _workContextAccessor.GetContext(filterContext).Layout;
            if (!string.IsNullOrWhiteSpace(sc.TopHeaderScript))
            {
                layout.BeforeHead = _shapeFactory.DocumentZone(ZoneName: "BeforeHead");
                layout.BeforeHead.Add(_shapeFactory.PageScript(ScriptContent: sc.TopHeaderScript));
            }
            if (!string.IsNullOrWhiteSpace(sc.BottomHeaderScript))
            {
                layout.Head.Add(_shapeFactory.PageScript(ScriptContent: sc.BottomHeaderScript), "100:after");
            }
            if (!string.IsNullOrWhiteSpace(sc.TopBodyScript))
            {
                layout.BeforeBody = _shapeFactory.DocumentZone(ZoneName: "BeforeBody");
                layout.BeforeBody.Add(_shapeFactory.PageScript(ScriptContent: sc.TopBodyScript));
            }

            if (!string.IsNullOrWhiteSpace(sc.BottomBodyScript))
            {
                layout.AfterTail = _shapeFactory.DocumentZone(ZoneName: "AfterTail");
                layout.AfterTail.Add(_shapeFactory.PageScript(ScriptContent: sc.BottomBodyScript));
            }
        }
    }
}
