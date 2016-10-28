using Orchard.Autoroute.Models;
using Orchard.Events;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Tokens
{
    public interface ITokenProvider : IEventHandler
    {
        void Describe(dynamic context);
        void Evaluate(dynamic context);
    }

    public class ContentTokens : ITokenProvider
    {
        public ContentTokens()
        {
        }

        public Localizer T { get; set; }

        public void Describe(dynamic context)
        {
            context.For("Autoroute", T("Autoroute alias"), T("Token for Autoroute part"))
                .Token("Alias", T("Display alias"), T("Full app-relative path to content"))
                .Token("LocalAlias", T("Local alias"), T("Local alias - relative to parent content"));
        }

        public void Evaluate(dynamic context)
        {
            context.For<AutoroutePart>("Autoroute")
                .Token("Alias", (Func<AutoroutePart, object>)(part => part.DisplayAlias))
                .Token("LocalAlias", (Func<AutoroutePart, object>)(part => part.LocalAlias));
        }
    }
}