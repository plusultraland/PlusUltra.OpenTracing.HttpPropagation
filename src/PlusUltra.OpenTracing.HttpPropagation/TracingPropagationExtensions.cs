using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using PlusUltra.OpenTracing.HttpPropagation.Incoming;
using PlusUltra.OpenTracing.HttpPropagation.Outgoing;
using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class TracingPropagationExtensions
    {
        public static IServiceCollection AddHttpTracingPropagation(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services
                .AddIncomingHttpRequestTracing()
                .AddOutgoingHttpRequestTracing();

            return services;
        }

        public static IServiceCollection AddIncomingHttpRequestTracing(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Register a IStartupFilter that will automatically add the IncomingTraceMiddleware to the
            // Kestrel's http pipeline. The IncomingTraceMiddleware will extract the tracing context of
            // each incoming request and create a local span as a child of it.
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStartupFilter, IncomingTraceStartupFilter>());

            return services;
        }

        public static IServiceCollection AddOutgoingHttpRequestTracing(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Register the IHttpMessageHandlerBuilderFilter that will inject the OutgoingTraceBuilderFilter
            // on all HttpClients created.
            // This will ensure that each outgoing request gets its own span and that it is injected
            // as headers to be retrieved by the remote server.
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, OutgoingTraceBuilderFilter>());

            return services;
        }
    }
}