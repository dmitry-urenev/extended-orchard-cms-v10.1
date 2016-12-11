using Orchard.Alias.Records;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orchard.StaticPages.Records
{
    public class RouteRecord
    {
        public virtual int Id { get; set; }
        public virtual string Path { get; set; }
        public virtual ActionRecord Action { get; set; }
        public virtual string RouteValues { get; set; }
    }
}