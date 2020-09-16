using Microsoft.Extensions.Http;
using OpenTracing;
using System;

namespace PlusUltra.OpenTracing.HttpPropagation.Outgoing
{
    public class OutgoingTraceBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        public OutgoingTraceBuilderFilter(ITracer tracer)
        {
            _tracer = tracer;
        }

        private readonly ITracer _tracer;

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));

            return builder =>
            {
                next(builder);

                builder.AdditionalHandlers.Add(new OutgoingTraceRequestHandler(_tracer));
            };
        }
    }
}