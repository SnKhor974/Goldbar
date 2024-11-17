using Polly;
using Polly.Extensions.Http;

namespace Extensions.Polly
{
    public static class PollyRetryPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> Policy(IServiceProvider provider, HttpRequestMessage message)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.IsSuccessStatusCode != true)
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                });
        }
    }
}
