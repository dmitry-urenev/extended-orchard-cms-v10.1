using Orchard.ContentManagement;
using Orchard.Core.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Models
{
    public class ExMenuItemEntry : MenuItemEntry
    {
        public bool IsOriginal { get; set; }

        public string Culture { get; set; }

        public int? CultureId { get; set; }
    }
}