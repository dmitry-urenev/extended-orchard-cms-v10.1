using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Aliases
{
    public class AliasBuildContext
    {
        public string Alias { get; set; }

        public List<AliasSegment> Segments { get; set; }

        public bool Canceled { get; set; }
    }

    public static class Extentions
    {
        public static string Get(this List<AliasSegment> segments, string token)
        {
            return segments.Where(s => s.Token.Equals(token, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Value)
                .SingleOrDefault();
        }

        public static void Set(this List<AliasSegment> segments, string token, string value)
        {
            var seg = segments.FirstOrDefault(s => s.Token.Equals(token, StringComparison.OrdinalIgnoreCase));
            if (seg != null)
            {
                seg.Value = value;
            }
            else
            {
                segments.Add(new AliasSegment { Token = token, Value = value });
            }
        }
    }
}