using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UCenter.SDK
{
    public class UCenterHttpClient
    {
        public async Task<TResponse> SendAsync<TResponse>(string url, ObjectContent content)
            where TResponse : UCenterResponse
        {
            TResponse result = default(TResponse);
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    var request = CreateHttpRequest(url, content);
                    var response = await httpClient.SendAsync(request);
                    result = await ParseResponseAsync<TResponse>(response);
                }

                if (result != null && result.Status == UCenterResponseStatus.Success)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public async Task<TResult> SendAsyncWithException<TResponse, TResult>(string url, ObjectContent content) where TResponse : UCenterResponse
        {
            var response = await this.SendAsync<TResponse>(url, content);
            if (response.Status == UCenterResponseStatus.Success)
            {
                return response.As<TResult>();
            }
            else
            {
                if (response.Error != null)
                {
                    throw new ApplicationException($"[{{response.Error.Code}}]:{{response.Error.Message}}");
                }
                throw new ApplicationException("Unknown exception");
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

        private HttpRequestMessage CreateHttpRequest(string url, ObjectContent content)
        {
            var method = HttpMethod.Post;
            var uri = BuildUri(url);
            var message = new HttpRequestMessage(method, uri);
            message.Headers.Clear();
            message.Headers.ExpectContinue = false;
            message.Content = content;
            return message;
        }

        private Uri BuildUri(string url)
        {
            return new Uri(url);
        }

        private async Task<TResponse> ParseResponseAsync<TResponse>(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("++++++++++++RESPONSE STRING+++++++++++");
            Debug.WriteLine(content);
            var result = JsonConvert.DeserializeObject<TResponse>(content);
            return result;
        }
    }
}
