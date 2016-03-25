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
                    throw UCenterExceptionManager.FromError(new UCenterError
                    {
                        ErrorCode = response.Error.ErrorCode,
                        Message = response.Error.Message
                    });
                }
                else
                {
                    throw UCenterExceptionManager.FromError(new UCenterError
                    {
                        ErrorCode = UCenterErrorCode.Failed,
                        Message = "Error occurred when sending http request"
                    });
                }
            }
        }

        public async Task<TResult> SendMutipleContent<TContent, TResult>(HttpMethod method, string url, TContent content)
        {
            using (var client = new HttpClient())
            {
                using (var muLtipart = new MultipartFormDataContent())
                {
                    muLtipart.Add(new ObjectContent<TContent>(content, new JsonMediaTypeFormatter()));

                    string fileName = @"c:\git\UCenter\src\GF.UCenter.Test\TestData\github.png";
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(fileName));
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "Foo.txt"
                    };
                    muLtipart.Add(fileContent);

                    var response = await client.PostAsync(url, muLtipart);
                    var result = await response.Content.ReadAsAsync<UCenterResponse<TResult>>();
                    return result.Content;
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
