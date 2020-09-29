using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;

namespace PlusUltra.OpenTracing.HttpPropagation.Outgoing
{
    public class OutgoingTraceRequestHandler : DelegatingHandler
    {
        public OutgoingTraceRequestHandler(ITracer tracer)
        {
            _tracer = tracer;
        }

        private readonly ITracer _tracer;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var spanBuilder = _tracer.BuildSpan($"Fetch HTTP {request.Method.Method} {request.RequestUri.GetLeftPart(UriPartial.Path)}")
                .WithTag(Tags.SpanKind, Tags.SpanKindClient)
                .WithTag(Tags.HttpMethod, request.Method.ToString())
                .WithTag(Tags.HttpUrl, request.RequestUri.ToString())
                .WithTag(Tags.PeerHostname, request.RequestUri.Host)
                .WithTag(Tags.PeerPort, request.RequestUri.Port);

            using var scope = spanBuilder.StartActive();

            _tracer.Inject(scope.Span.Context, BuiltinFormats.HttpHeaders, new HttpHeadersInjectAdapter(request.Headers));

            try
            {
                var result = await base.SendAsync(request, cancellationToken);

                Tags.HttpStatus.Set(scope.Span, (int)result.StatusCode);

                if (!result.IsSuccessStatusCode)
                {
                    Tags.Error.Set(scope.Span, true);
                }

                return result;
            }
            catch (Exception ex)
            {
                Tags.Error.Set(scope.Span, true);
                scope.Span.SetTag("http.exception", ex.Message);

                throw;
            }
        }
    }
}