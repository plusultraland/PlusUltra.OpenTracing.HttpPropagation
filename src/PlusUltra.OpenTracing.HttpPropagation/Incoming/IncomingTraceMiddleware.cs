using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using PlusUltra.OpenTracing.HttpPropagation.Incoming.Configuration;

namespace PlusUltra.OpenTracing.HttpPropagation.Incoming
{
    public class IncomingTraceMiddleware
    {
        private static readonly string NoHostSpecified = string.Empty;

        private readonly RequestDelegate _next;

        public IncomingTraceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            var options = context.RequestServices.GetService<IOptions<IncomingTraceOptions>>().Value;
            var shouldIgnore = options.ShouldIgnoreUrl(request.Path);
            if (shouldIgnore)
            {
                await _next(context);
                return;
            }

            var tracer = context.RequestServices.GetService<ITracer>();
            var extractedSpanContext = tracer.Extract(BuiltinFormats.HttpHeaders, new RequestHeadersExtractAdapter(request.Headers));

            var spanBuilder = tracer.BuildSpan($"Serve HTTP {request.Method} {request.Path}")
                .AsChildOf(extractedSpanContext)
                .WithTag(Tags.SpanKind, Tags.SpanKindServer)
                .WithTag(Tags.HttpMethod, request.Method)
                .WithTag(Tags.HttpUrl, GetDisplayUrl(request))
                .WithTag("operation.id", System.Diagnostics.Activity.Current.RootId);

            using var scope = spanBuilder.StartActive();

            try
            {
                await _next(context);

                Tags.HttpStatus.Set(scope.Span, context.Response.StatusCode);

                if (context.Response.StatusCode > 399)
                {
                    Tags.Error.Set(scope.Span, true);
                }
            }
            catch (Exception ex)
            {
                Tags.Error.Set(scope.Span, true);
                scope.Span.SetTag("http.exception", ex.Message);

                throw;
            }
        }

        private static string GetDisplayUrl(HttpRequest request)
        {
            if (request.Host.HasValue)
            {
                return request.GetDisplayUrl();
            }

            // HTTP 1.0 requests are not required to provide a Host to be valid
            // Since this is just for display, we can provide a string that is
            // not an actual Uri with only the fields that are specified.
            // request.GetDisplayUrl(), used above, will throw an exception
            // if request.Host is null.
            return $"{request.Scheme}://{NoHostSpecified}{request.PathBase.Value}{request.Path.Value}{request.QueryString.Value}";
        }
    }
}