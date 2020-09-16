using System;
using System.Collections.Generic;
using System.Linq;

namespace PlusUltra.OpenTracing.HttpPropagation.Incoming.Configuration
{
    public class IncomingTraceOptions
    {
        private readonly List<string> _ignoreUrls = new List<string>();

        public void AddIgnoreUrl(string url) => _ignoreUrls.Add(url);

        public bool ShouldIgnoreUrl(string url) => _ignoreUrls.Any(x => x.Equals(url, StringComparison.InvariantCultureIgnoreCase));
    }
}

