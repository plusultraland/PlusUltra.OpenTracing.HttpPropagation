using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using OpenTracing.Util;

namespace PlusUltra.OpenTracing.HttpPropagation.Filters
{
    public class TraceIncomingRequestFilter : IAsyncActionFilter
    {
        internal static readonly string NoHostSpecified = String.Empty;

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

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var action = context.ActionDescriptor;
            var request = context.HttpContext.Request;

            ISpanContext extractedSpanContext = GlobalTracer.Instance.Extract(BuiltinFormats.HttpHeaders, new RequestHeadersExtractAdapter(request.Headers));

            var spanBuilder = GlobalTracer.Instance.BuildSpan(request.Method)
                .AsChildOf(extractedSpanContext)
                .WithTag(Tags.SpanKind, Tags.SpanKindServer)
                .WithTag(Tags.HttpMethod, request.Method)
                .WithTag(Tags.HttpUrl, GetDisplayUrl(request));

            using (var scope = spanBuilder.StartActive())
            {
                var resultContext = await next();
            }
        }
    }
}