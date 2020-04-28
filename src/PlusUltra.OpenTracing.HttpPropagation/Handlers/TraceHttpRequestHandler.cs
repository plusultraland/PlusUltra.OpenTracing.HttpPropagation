using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;

namespace PlusUltra.OpenTracing.HttpPropagation.Handlers
{
    public class TraceHttpRequestHandler : DelegatingHandler
    {
        public TraceHttpRequestHandler(ITracer tracer)
        {
            this.tracer = tracer;
        }

        private readonly ITracer tracer;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var spanBuilder = tracer.BuildSpan($"HTTP {request.Method.Method}")
                .WithTag(Tags.SpanKind, Tags.SpanKindClient)
                .WithTag(Tags.HttpMethod, request.Method.ToString())
                .WithTag(Tags.HttpUrl, request.RequestUri.ToString())
                .WithTag(Tags.PeerHostname, request.RequestUri.Host)
                .WithTag(Tags.PeerPort, request.RequestUri.Port);

            using (var scope = spanBuilder.StartActive())
            {
                tracer.Inject(scope.Span.Context, BuiltinFormats.HttpHeaders, new HttpHeadersInjectAdapter(request.Headers));
                var result = await base.SendAsync(request, cancellationToken);

                scope.Span.SetTag(Tags.HttpStatus, (int)result.StatusCode);

                if (!result.IsSuccessStatusCode)
                    Tags.Error.Set(scope.Span, true);

                return result;
            }
        }
    }
}