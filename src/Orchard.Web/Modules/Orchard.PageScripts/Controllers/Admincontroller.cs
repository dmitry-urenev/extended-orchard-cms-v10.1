using Orchard.PageScripts.Models;
using Orchard.PageScripts.Services;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Orchard.PageScripts.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IPageScriptService _scriptService;

        public AdminController(IPageScriptService scriptService)
        {
            _scriptService = scriptService;
        }

        public ActionResult Index()
        {
            var sc = _scriptService.GetScripts();
            return View(sc);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Index(PageScriptsInfo model)
        {
            var sc = _scriptService.GetScripts();
            sc.TopBodyScript = model.TopBodyScript;
            sc.TopHeaderScript = model.TopHeaderScript;
            sc.BottomBodyScript = model.BottomBodyScript;
            sc.BottomHeaderScript = model.BottomHeaderScript;
            _scriptService.Update(sc);

            return RedirectToAction("Index");
        }
    }
}
