using Orchard.Alias;
using Orchard.Autoroute.Models;
using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.ExContentManagement.Aliases;
using Orchard.ExContentManagement.Models;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Handlers
{
    public class AutorouteAliasHandler : ContentHandler
    {
        private readonly Lazy<IAutorouteService> _autorouteService;
        private readonly IOrchardServices _orchardServices;
        private IAliasService _aliasService;
        private ICultureManager _cultureManager;
        private Orchard.ExContentManagement.Aliases.IAliasFactory _aliasFactory;

        private string autorouteAliasSource = "Autoroute:View";

        public Localizer T { get; set; }

        public AutorouteAliasHandler(
            IRepository<AutoroutePartRecord> autoroutePartRepository,
            Lazy<IAutorouteService> autorouteService,
            IOrchardServices orchardServices,
            ICultureManager cultureManager,
            IAliasService aliasService,
            Orchard.ExContentManagement.Aliases.IAliasFactory aliasFactory)
        {
            _aliasService = aliasService;
            _autorouteService = autorouteService;
            _orchardServices = orchardServices;
            _cultureManager = cultureManager;
            _aliasFactory = aliasFactory;

            OnPublished<AutoroutePart>((ctx, part) => ProcessAlias(part));            
        }

        private string GetSlug(string alias)
        {
            if (alias == "/")
                return "";

            var slug = alias.Trim('/');
            var idx = alias.LastIndexOf("/");
            if (idx != -1)
            {
                slug = alias.Substring(idx + 1);
            }
            return slug;
        }

        public void TrimParentPath(AutoroutePart part)
        {
            if (part.Has<ParentContentPart>())
            {
                var parent = part.As<ParentContentPart>().ParentContent;
                var parentPath = parent.As<AutoroutePart>().DisplayAlias;

                TrimParentPath(part, parentPath);
            }
        }

        private void TrimParentPath(AutoroutePart part, string parentPath)
        {
            var alias = part.DisplayAlias;

            if (alias.IndexOf(parentPath) == 0)
            {
                alias = alias.Substring(parentPath.Length).TrimStart('/');
            }
            part.DisplayAlias = alias;
        }      

        private void PublishAlias(AutoroutePart part)
        {
            if (part.DisplayAlias.StartsWith("/"))
                part.DisplayAlias = part.DisplayAlias.TrimStart('/');

            var previous = part.DisplayAlias;
            if (!_autorouteService.Value.ProcessPath(part))
            {
                _orchardServices.Notifier.Warning(T("Permalinks in conflict. \"{0}\" is already set for a previously created {2} so now it has the slug \"{1}\"",
                                             previous, part.Path, part.ContentItem.ContentType));
            }
            _autorouteService.Value.PublishAlias(part);
        }

        private void ProcessAlias(AutoroutePart part)
        {
            if (part.DisplayAlias != "/")
            {              
                var aliasData = _aliasFactory.BuidFor(part);
                string alias = aliasData.Alias;
                if (!string.Equals(part.DisplayAlias, alias, StringComparison.OrdinalIgnoreCase))
                {
                    RemoveAlias(part.DisplayAlias);
                    part.DisplayAlias = alias;
                    PublishAlias(part);

                    // update child paths
                    var childRoutes = _orchardServices.ContentManager
                        .Query<CommonPart, CommonPartRecord>(VersionOptions.Published)
                        .Where(c => c.Container != null && c.Container.Id == part.ContentItem.Id)
                        .List()
                        .Where(p => p.Has<AutoroutePart>())
                        .ToList();

                    foreach (var child in childRoutes)
                    {
                        var aPart = child.As<AutoroutePart>();
                        var childAliasData = _aliasFactory.BuidFor(aPart);

                        aPart.DisplayAlias = childAliasData.Alias;
                        PublishAlias(aPart);
                    }
                }
            }
        }        

        /// <summary>
        /// Remove alias for all sources
        /// </summary>
        /// <param name="alias"></param>
        private void RemoveAlias(string alias)
        {
            alias = alias.TrimStart('/'); 
            _aliasService.Delete(alias, autorouteAliasSource);
        }
    }
}