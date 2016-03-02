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
        public async Task<TResponse> PostAsync<TResponse>(string url, ObjectContent content)
        {

            TResponse result = default(TResponse);
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    var request = CreateHttpRequest(url, content);
                    var response = await httpClient.SendAsync(request);
                    Debug.WriteLine("===========Debug========");
                    Debug.WriteLine(response.Content.ReadAsStringAsync());

                    result = await ParseResponseAsync<TResponse>(response);
                }
            }
            catch (Exception ex)
            {
            }
            return result;
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
            return JsonConvert.DeserializeObject<TResponse>(content);
        }
    }
}
