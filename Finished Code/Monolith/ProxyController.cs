using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Wrap;

namespace Monolith
{
    [Route("[action]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncFallbackPolicy<IActionResult> _fallbackPolicy;
        private readonly AsyncRetryPolicy<IActionResult> _retryPolicy;
        private static AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly AsyncPolicyWrap<IActionResult> _policy;

        public ProxyController(IHttpClientFactory httpClientFactory)
        {
            _fallbackPolicy = Policy<IActionResult>
                .Handle<Exception>()
                .FallbackAsync(Content("Sorry, we are currently experiencing issues. Please try again later"));

            _retryPolicy = Policy<IActionResult>
                .Handle<Exception>()
                .RetryAsync();

            if (_circuitBreakerPolicy == null)
            {
                _circuitBreakerPolicy = Policy
                    .Handle<Exception>()
                    .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
            }

            _policy = Policy<IActionResult>
                .Handle<Exception>()
                .FallbackAsync(Content("Sorry, we are currently experiencing issues. Please try again later"))
                .WrapAsync(_retryPolicy)
                .WrapAsync(_circuitBreakerPolicy);

            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> Books()
            => await ProxyTo("https://localhost:6001/books");

        [HttpGet]
        public async Task<IActionResult> Authors()
            => await ProxyTo("https://localhost:5001/authors");

        private async Task<IActionResult> ProxyTo(string url)
            => await _policy.ExecuteAsync(async () => Content(await _httpClient.GetStringAsync(url)));
    }
}
