using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Orchard.Alias;
using Orchard.Autoroute.Models;
using Orchard.Autoroute.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Tokens;
using Orchard.Localization.Services;
using Orchard.Mvc;
using System.Web;
using Orchard.ContentManagement.Aspects;

namespace Orchard.Autoroute.Services {
    public class AutorouteService : Component, IAutorouteService
    {

        private readonly IAliasService _aliasService;
        private readonly IAliasPatternProvider _patternProvider;

        private readonly ITokenizer _tokenizer;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly IRouteEvents _routeEvents;
        private readonly ICultureManager _cultureManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string AliasSource = "Autoroute:View";

        public AutorouteService(
            IAliasService aliasService,
            IAliasPatternProvider patternProvider,
            ITokenizer tokenizer,
            IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager,
            IRouteEvents routeEvents,
            IHttpContextAccessor httpContextAccessor)
        {

            _aliasService = aliasService;
            _patternProvider = patternProvider;
            _tokenizer = tokenizer;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _routeEvents = routeEvents;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateAlias(AutoroutePart part)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }

            var pattern = _patternProvider.GetPatternFor(part);

            // Convert the pattern and route values via tokens.
            return GenerateAliasByPattern(part, pattern);
        }

        private string GenerateAliasByPattern(AutoroutePart part, string pattern)
        {
            // Convert the pattern and route values via tokens.
            var path = _tokenizer.Replace(pattern, BuildTokenContext(part.ContentItem), new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });

            Regex rgx = new Regex("/{2,}");
            string result = rgx.Replace(path, "/");
            path = result.Trim('/').ToLower();

            return path;
        }

        public string GenerateLocalAlias(AutoroutePart part)
        {
            if (part == null)
                throw new ArgumentNullException("part");

            string pattern = "{Content.Slug}";
            return GenerateAliasByPattern(part, pattern);
        }


        public void PublishAlias(AutoroutePart part)
        {
            var displayRouteValues = _contentManager.GetItemMetadata(part).DisplayRouteValues;

            _aliasService.Replace(part.DisplayAlias, displayRouteValues, AliasSource, true);
            if (part.IsHomePage)
            {
                _aliasService.Set("", displayRouteValues, AliasSource);
            }

            _routeEvents.Routed(part, part.DisplayAlias);
        }

        private IDictionary<string, object> BuildTokenContext(IContent item)
        {
            return new Dictionary<string, object> { { "Content", item } };
        }

        public void RemoveAliases(AutoroutePart part)
        {
            _aliasService.Delete(part.Path, AliasSource);
        }

        public string GenerateUniqueSlug(AutoroutePart part, IEnumerable<string> existingPaths)
        {
            if (existingPaths == null || !existingPaths.Contains(part.Path))
                return part.Path;

            var version = existingPaths.Select(s => GetSlugVersion(part.Path, s)).OrderBy(i => i).LastOrDefault();

            return version != null
                ? String.Format("{0}-{1}", part.Path, version)
                : part.Path;
        }

        public IEnumerable<AutoroutePart> GetSimilarPaths(string path)
        {
            return
                _contentManager.Query<AutoroutePart, AutoroutePartRecord>()
                    .Where(part => part.DisplayAlias != null && part.DisplayAlias.StartsWith(path))
                    .List();
        }

        public bool IsPathValid(string slug)
        {
            return String.IsNullOrWhiteSpace(slug) || 
                Regex.IsMatch(slug, @"^[^:?#\[\]@!$&'()+,;=\s\""\<\>\\\|%]+$");     // *. removed
        }

        public bool ProcessPath(AutoroutePart part)
        {
            var pathsLikeThis = GetSimilarPaths(part.Path).ToArray();

            // Don't include *this* part in the list
            // of slugs to consider for conflict detection.
            pathsLikeThis = pathsLikeThis.Where(p => p.ContentItem.Id != part.ContentItem.Id).ToArray();

            //if (pathsLikeThis.Any())
            //{
            //    var originalPath = part.Path;
            //    var newPath = GenerateUniqueSlug(part, pathsLikeThis.Select(p => p.Path));
            //    part.DisplayAlias = newPath;

            //    if (originalPath != newPath)
            //        return false;
            //}

            if (pathsLikeThis.Any())
            {
                var originalPath = part.DisplayAlias;
                var url = originalPath.TrimEnd(part.LocalAlias.ToArray());
                var newAlias = GenerateUniqueSlug(part, pathsLikeThis.Select(p => p.Path));
                var newLocalAlias = newAlias.Substring(url.Length).TrimStart('/');

                part.DisplayAlias = newAlias;
                part.LocalAlias = newLocalAlias;

                if (originalPath != newAlias)
                    return false;
            }

            return true;
        }

        private static int? GetSlugVersion(string path, string potentialConflictingPath)
        {
            int v;
            var slugParts = potentialConflictingPath.Split(new[] { path }, StringSplitOptions.RemoveEmptyEntries);

            if (slugParts.Length == 0)
                return 2;

            return Int32.TryParse(slugParts[0].TrimStart('-'), out v)
                ? (int?)++v
                : null;
        }

        public void CreatePattern(string contentType, string name, string pattern, string description, bool makeDefault)
        {
            _patternProvider.CreatePattern(contentType, name, pattern, description, makeDefault);
        }

        public RoutePattern GetDefaultPattern(string contentType, string culture)
        {
            return _patternProvider.GetDefaultPattern(contentType, culture);
        }

        public IEnumerable<RoutePattern> GetPatterns(string contentType)
        {
            return _patternProvider.GetPatterns(contentType);
        }
    }
}
