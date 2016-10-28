using System.Collections.Generic;
using Orchard.ContentManagement;
using System.Linq;
using System;

namespace Orchard.Localization.Models
{
    public class CultureSwitcherPart : ContentPart<CultureSwitcherPartRecord>
    {
        public int DisplayMode
        {
            get { return Record.DisplayMode; }
            set { Record.DisplayMode = value; }
        }

        public int DisplayType
        {
            get { return Record.DisplayType; }
            set { Record.DisplayType = value; }
        }

        public IEnumerable<string> CultureOrderedList
        {
            get
            {
                return (Record.OrderedCultures ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            set
            {
                if (value != null && value.Any())
                    Record.OrderedCultures = string.Join(";", value);
                else
                    Record.OrderedCultures = string.Empty;
            }
        }

        public IEnumerable<string> AvailableCultures { get; set; }

        public string UserCulture { get; set; }
    }
}