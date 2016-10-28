using Orchard.Autoroute.Models;
using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Aliases
{
    public class DefaultAliasFactory : IAliasFactory
    {
        private readonly ITokenizer _tokenizer;
        private readonly IAliasPatternProvider _patternProvider;

        public DefaultAliasFactory(
            ITokenizer tokenizer,
            IAliasPatternProvider patternProvider)
        {
            _tokenizer = tokenizer;
            _patternProvider = patternProvider;
        }

        public AliasBuildResult BuidFor(IContent contentItem)
        {
            var context = new Dictionary<string, object> { { "Content", contentItem } };

            var pattern = _patternProvider.GetPatternFor(contentItem);
            if (contentItem.Has<AutoroutePart>() && pattern.IndexOf("{Content.Slug}", StringComparison.OrdinalIgnoreCase) != -1)
            {
                pattern = pattern.Replace("{Content.Slug}", "{Autoroute.LocalAlias}");
                context.Add("Autoroute", contentItem.As<AutoroutePart>());
            }

            var tokens = Parse(pattern, false).Item2;

            // Convert the pattern and route values via tokens
            var path = _tokenizer.Replace(pattern, context, new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
            // removing trailing slashes in case the container is empty, and tokens are base on it (e.g. home page)
            path = (path ?? "").Trim('/');

            var segments = _tokenizer.Evaluate(tokens, context);

            var result = new AliasBuildResult()
            {
                Alias = path,
                Segments = new List<AliasSegment>()
            };

            foreach (var s in segments)
            {
                switch (s.Key)
                {
                    case "Content.Culture":
                        result.Segments.Add(new AliasSegment("{Culture}", ((string)s.Value).Trim('/')));
                        break;
                    case "Content.ParentPath":
                        result.Segments.Add(new AliasSegment("{Parent}", ((string)s.Value).Trim('/')));
                        break;
                    case "Autoroute.LocalAlias":
                        result.Segments.Add(new AliasSegment("{Slug}", ((string)s.Value).Trim('/')));
                        break;
                    case "Content.Slug":
                        result.Segments.Add(new AliasSegment("{Slug}", ((string)s.Value).Trim('/')));
                        break;
                }
            }
            return result;
        }

        private static Tuple<string, IEnumerable<string>> Parse(string text, bool hashMode)
        {
            var tokens = new List<string>();
            if (!String.IsNullOrEmpty(text))
            {
                var inToken = false;
                var tokenStart = 0;
                for (var i = 0; i < text.Length; i++)
                {
                    var c = text[i];

                    if (c == '{')
                    {
                        if (i + 1 < text.Length && text[i + 1] == '{')
                        {
                            text = text.Substring(0, i) + text.Substring(i + 1);
                            continue;
                        }
                    }
                    else if (c == '}' && !(inToken))
                    {
                        if (i + 1 < text.Length && text[i + 1] == '}')
                        {
                            text = text.Substring(0, i) + text.Substring(i + 1);
                            continue;
                        }
                    }

                    if (inToken)
                    {
                        if (c == '}')
                        {
                            inToken = false;
                            var token = text.Substring(tokenStart + 1, i - tokenStart - 1);
                            tokens.Add(token);
                        }
                    }
                    else if (!hashMode && c == '{')
                    {
                        inToken = true;
                        tokenStart = i;
                    }
                    else if (hashMode && c == '#'
                        && i + 1 < text.Length && text[i + 1] == '{'
                        && (i + 2 > text.Length || text[i + 2] != '{'))
                    {
                        inToken = true;
                        tokenStart = i + 1;
                    }
                }
            }
            return new Tuple<string, IEnumerable<string>>(text, tokens);
        }

        //IEnumerable<IAliasRule> _rules;

        //public DefaultAliasFactory(IEnumerable<IAliasRule> rules)
        //{
        //    _rules = rules;
        //}

        //public AliasBuildResult BuidFor(IContent contentItem)
        //{
        //    var context = new AliasBuildContext()
        //    {
        //        Alias = contentItem.As<IAliasAspect>().Path ?? "",
        //        Segments = new List<AliasSegment>()
        //    };

        //    foreach (var r in _rules.OrderBy(r => r.Priority))
        //    {
        //        if (r.Match(contentItem))
        //        {
        //            r.Apply(contentItem, context);
        //            if (context.Canceled)
        //                break;
        //        }
        //    }

        //    // return default path
        //    return new AliasBuildResult
        //    {
        //        Alias = context.Alias,
        //        Segments = context.Segments
        //    };
        //}



    }
}