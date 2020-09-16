using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace PlusUltra.OpenTracing.HttpPropagation.Incoming
{
    public class IncomingTraceStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<IncomingTraceMiddleware>();

                next(app);
            };
        }
    }
}
