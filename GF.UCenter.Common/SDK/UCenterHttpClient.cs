using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GF.UCenter.Common.Portable;

namespace GF.UCenter.Common
{
    public class UCenterHttpClient
    {
        public Task<TResponse> SendAsync<TContent, TResponse>(HttpMethod method, string url, TContent content)
        {
            HttpContent httpContent = null;
            if (content is HttpContent)
            {
                httpContent = content as HttpContent;
            }
            else
            {
                httpContent = new ObjectContent<TContent>(content, new JsonMediaTypeFormatter());
            }

            return this.SentAsync<TResponse>(method, url, httpContent);
        }

        public async Task<TResponse> SentAsync<TResponse>(HttpMethod method, string url, HttpContent content)
        {
            using (var httpClient = CreateHttpClient())
            {
                var request = new HttpRequestMessage(method, new Uri(url));
                request.Headers.Clear();
                request.Headers.ExpectContinue = false;
                request.Content = content;

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
                return response.Result;
            }
            else
            {
                if (response.Error != null)
                {
                    throw new UCenterException(response.Error.ErrorCode, response.Error.Message);
                }
                else
                {
                    throw new UCenterException(UCenterErrorCode.Failed, "Error occurred when sending http request");
                }
            }
        }

        public HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;

            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(90);

            return httpClient;
        }
    }
}
