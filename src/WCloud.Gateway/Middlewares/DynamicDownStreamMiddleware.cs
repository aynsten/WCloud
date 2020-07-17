using Ocelot.Logging;
using Ocelot.Middleware;
using System;
using System.Threading.Tasks;

namespace WCloud.Gateway.Middlewares
{
    [Obsolete]
    public class DynamicDownStreamMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;
        public DynamicDownStreamMiddleware(OcelotRequestDelegate _next, IOcelotLogger logger) : base(logger)
        {
            this._next = _next;
        }

        public async Task Invoke(DownstreamContext context)
        {



            await this._next.Invoke(context);
        }
    }
}
