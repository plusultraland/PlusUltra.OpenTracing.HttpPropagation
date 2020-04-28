using Microsoft.AspNetCore.Mvc;
using PlusUltra.OpenTracing.HttpPropagation.Filters;
using PlusUltra.OpenTracing.HttpPropagation.Handlers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureTracingExtensions
    {
        public static MvcOptions AddTraceIncomingRequestFilter(this MvcOptions options)
        {
            options.Filters.Add(new TraceIncomingRequestFilter());

            return options;
        }
    }
}