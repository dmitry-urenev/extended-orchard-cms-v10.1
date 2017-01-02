using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Localization
{
    public static class Signals
    {
        public static readonly object CurrentCultureChanged = new object();
    }
}
