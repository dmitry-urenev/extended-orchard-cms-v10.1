using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Aliases
{
    public class AliasSegment
    {
        public AliasSegment() { }
        public AliasSegment(string token, string sValue)
        {
            Token = token;
            Value = sValue;
        }
        public string Token { get; set; }

        public string Value { get; set; }        
    }
}