using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Localization.ViewModels
{
    public class AddLocalizationViewModel
    {
        public int Id { get; set; }

        [Required]
        public string SelectedCulture { get; set; }
        public IEnumerable<string> SiteCultures { get; set; }
        public IEnumerable<string> AvailableCultures { get; set; }
        public string SiteCulture { get; set; }
        public IContent Content { get; set; }
        public IContent OriginalContent { get; set; }
    }
}
