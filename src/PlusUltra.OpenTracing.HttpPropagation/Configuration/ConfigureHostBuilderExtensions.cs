using System;
using PlusUltra.OpenTracing.HttpPropagation.Handlers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Microsoft.AspNetCore.Hosting
{
    public static class ConfigureHostBuilderExtensions
    {
        public static IWebHostBuilder PropagateHttpTracingContext(this IWebHostBuilder hostBuilder)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            return hostBuilder.ConfigureServices((services) =>
            {
                services.TryAddTransient<TraceHttpRequestHandler>();

                services.AddSingleton<IHttpMessageHandlerBuilderFilter, TraceHttpRequestHandlerBuilderFilter>();
            });
        }
    }
}