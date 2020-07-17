using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WCloud.Framework.Middleware
{
    public class SetEncodingMiddleware : _BaseMiddleware
    {
        public SetEncodingMiddleware(RequestDelegate next) : base(next)
        { }

        public override async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
        }
    }
}
