using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.ExContentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Aliases.Rules
{
    public class ParentAliasRule : IAliasRule
    {
        private IContentManager _contentManager;

        public ParentAliasRule(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public bool Match(IContent contentItem)
        {
            return contentItem.Has<ParentContentPart>();
        }

        public void Apply(IContent contentItem, AliasBuildContext context)
        {
            string alias = context.Alias;

            if (contentItem.Has<ParentContentPart>())
            {
                IContent parentContent = contentItem.As<ParentContentPart>().ParentContent;
                string parentPath = parentContent != null ? parentContent.As<AutoroutePart>().DisplayAlias : null;
                string slug = "";

                if (parentContent != null && alias.StartsWith(parentPath))
                {
                    // parent == old parent 
                    slug = alias.Substring(parentPath.Length).Trim('/');
                    context.Segments.Set("{Slug}", slug);
                    context.Segments.Set("{Parent}", parentPath);
                    context.Canceled = true; // stop processing

                    context.Alias = alias;

                    return;
                }
                // parent == null || parent != old parent 
                IContent prevParent = null;
                string prevParentPath = null;

                var versionNumber = contentItem.ContentItem.Version - 1;
                if (versionNumber > 0)
                {
                    var version = _contentManager.Get(contentItem.Id, VersionOptions.Number(versionNumber), 
                        new QueryHints().ExpandRecords<ParentContentPartRecord>());                    

                    if (version != null && version.Has<ParentContentPart>())
                    {
                        prevParent = _contentManager.Get(version.As<ParentContentPart>().ParentId, VersionOptions.Latest);
                        prevParentPath = prevParent != null ? prevParent.As<AutoroutePart>().DisplayAlias : null;

                        if (parentContent != prevParent &&
                            prevParent != null && alias.StartsWith(prevParentPath))
                        {
                            slug = alias.Substring(prevParentPath.Length).Trim('/');
                            context.Segments.Set("{Slug}", slug);

                            if (parentContent != null)
                            {
                                alias = parentPath + "/" + slug;
                                context.Segments.Set("{Parent}", parentPath);
                            }
                            else
                                alias = slug;

                            context.Canceled = true;
                            context.Alias = alias;
                            return;
                        }
                    }
                }

                // (parent == null || parent != old parent ) && (old parent == null || !alias.StartWith(old parent path))
                // default case:
                slug = alias;
                context.Segments.Set("{Slug}", alias);

                if (parentContent != null)
                {
                    alias = parentPath + "/" + slug;
                    context.Segments.Set("{Parent}", parentPath);
                    context.Canceled = true;
                }
                else
                    alias = slug;

                context.Alias = alias;                
            }
            else
            {
                context.Alias = alias;
            }
        }

        public string Rule
        {
            get { return "{Parent}/{Slug}"; }
        }

        public int Priority
        {
            get { return 5; }
        }
    }
}