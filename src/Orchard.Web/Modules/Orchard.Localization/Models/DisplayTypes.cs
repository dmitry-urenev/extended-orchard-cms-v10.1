using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Localization.Models
{
    public enum DisplayTypes
    {
        Culture = 1,
        Language = 2,
        NativeLanguage = 3,
        LanguageWithCountry = 4,
        NativeLanguageWithCountry = 5,
        TwoLetterLanguage = 6
    }
}