using System;
using System.Collections.Generic;
using Orchard;
using Orchard.SEO.Models;

namespace Orchard.SEO.Services {
	public interface IRobotsService : IDependency {
		RobotsFileRecord Get();
		Tuple<bool, IEnumerable<string>> Save(string text);
	}
}