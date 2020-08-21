using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Net.Http;
using WCloud.Core;

namespace WCloud.Member.InternalApi.Client
{
    public static class MemberInternalApiHttpClientExtension
    {
        public const string HTTP_CLIENT_NAME = "member-internal-api";

        public static IServiceCollection AddMemberInternalApiHttpClient(this IServiceCollection services, IConfiguration config)
        {
            var api_gateway_address = config.GetInternalApiGatewayAddressOrThrow();
            var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(3));

            var member_service_address = $"{api_gateway_address.TrimEnd('/', '\\')}/internal-api/member";

            services.AddHttpClient(HTTP_CLIENT_NAME, option =>
            {
                option.BaseAddress = new Uri(member_service_address);
            }).AddPolicyHandler(timeout);
            return services;
        }

        public static HttpClient GetMemberInternalApiHttpClient(this IServiceProvider provider)
        {
            var factory = provider.Resolve_<IHttpClientFactory>();
            var res = factory.CreateClient(HTTP_CLIENT_NAME);
            return res;
        }
    }
}
