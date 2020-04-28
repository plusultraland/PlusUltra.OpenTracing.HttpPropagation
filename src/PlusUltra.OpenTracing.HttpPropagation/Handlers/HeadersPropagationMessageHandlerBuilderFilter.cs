using System;
using Microsoft.Extensions.Http;
using OpenTracing;

namespace PlusUltra.OpenTracing.HttpPropagation.Handlers
{
    public class TraceHttpRequestHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        public TraceHttpRequestHandlerBuilderFilter(ITracer tracer)
        {
            this.tracer = tracer;
        }

        private readonly ITracer tracer;

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            return (builder) =>
            {
                next(builder);

                builder.AdditionalHandlers.Add(new TraceHttpRequestHandler(tracer));
            };
        }
    }
}