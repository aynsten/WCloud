using Lib.core;
using System.Net.Http;

namespace Lib.net
{
    public class HttpClientManager : StaticClientManager<HttpClient>
    {
        public static readonly HttpClientManager Instance = new HttpClientManager();

        public override string DefaultKey => "default";

        public override bool CheckClient(HttpClient ins)
        {
            return ins != null;
        }

        public override HttpClient CreateNewClient(string key)
        {
            return new HttpClient();
        }

        public override void DisposeClient(HttpClient ins)
        {
            ins?.Dispose();
        }
    }
}
