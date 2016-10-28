using Orchard.Core.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ExContentManagement.ViewModels
{
    public class NavigationAdminViewModel : NavigationManagementViewModel
    {
        public bool IsMultyLanguageSite { get; set; }
        public IEnumerable<string> AvailableCultures { get; set; }
        public string CurrentCulture { get; set; }
    }
}
