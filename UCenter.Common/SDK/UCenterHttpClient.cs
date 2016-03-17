using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UCenter.Common.SDK
{
    public class UCenterHttpClient
    {
        public async Task<TResponse> SendAsync<TContent, TResponse>(HttpMethod method, string url, TContent content)
        {
            using (var httpClient = CreateHttpClient())
            {
                var request = new HttpRequestMessage(method, new Uri(url));
                request.Headers.Clear();
                request.Headers.ExpectContinue = false;
                request.Content = new ObjectContent<TContent>(content, new JsonMediaTypeFormatter());

                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<TResponse>();
            }
        }

        public async Task<TResult> SendAsyncWithException<TContent, TResult>(HttpMethod method, string url, TContent content)
        {
            var response = await this.SendAsync<TContent, UCenterResponse<TResult>>(method, url, content);
            if (response.Status == UCenterResponseStatus.Success)
            {
                return response.Content;
            }
            else
            {
                if (response.Error != null)
                {
                    throw new ApplicationException($"[{response.Error.Code}]:{response.Error.Message}");
                }
                else
                {
                    throw new ApplicationException("Unknown exception");
                }
            }
        }

        public HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            handler.AutomaticDecompression = DecompressionMethods.GZip;

            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(100);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            return httpClient;
        }
    }
}
