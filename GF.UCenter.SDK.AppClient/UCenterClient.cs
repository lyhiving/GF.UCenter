namespace GF.UCenter.SDK.AppClient
{
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Common.Portable;
    using Common.SDK;

    public class UCenterClient
    {
        private readonly string host;
        private readonly UCenterHttpClient httpClient;

        public UCenterClient(string host)
        {
            this.httpClient = new UCenterHttpClient();
            this.host = host;
        }

        public async Task<AccountRegisterResponse> AccountRegisterAsync(AccountRegisterInfo info)
        {
            var url = GenerateApiEndpoint("account", "register");
            var response =
                await
                    httpClient.SendAsyncWithException<AccountRegisterInfo, AccountRegisterResponse>(HttpMethod.Post, url,
                        info);
            return response;
        }

        public async Task<AccountLoginResponse> AccountLoginAsync(AccountLoginInfo info)
        {
            var url = GenerateApiEndpoint("account", "login");
            var response =
                await
                    httpClient.SendAsyncWithException<AccountLoginInfo, AccountLoginResponse>(HttpMethod.Post, url, info);
            return response;
        }

        public async Task<AccountGuestLoginResponse> AccountGuestLoginAsync()
        {
            var url = GenerateApiEndpoint("account", "guest");
            var response =
                await
                    httpClient.SendAsyncWithException<AccountLoginInfo, AccountGuestLoginResponse>(HttpMethod.Post, url,
                        null);
            return response;
        }

        public async Task<AccountConvertResponse> AccountConvertAsync(AccountConvertInfo info)
        {
            var url = GenerateApiEndpoint("account", "convert");
            var response =
                await
                    httpClient.SendAsyncWithException<AccountConvertInfo, AccountConvertResponse>(HttpMethod.Post, url,
                        info);
            return response;
        }

        public async Task<AccountResetPasswordResponse> AccountResetPasswordAsync(AccountResetPasswordInfo info)
        {
            var url = this.GenerateApiEndpoint("account", "resetpassword");
            return
                await
                    httpClient.SendAsyncWithException<AccountResetPasswordInfo, AccountResetPasswordResponse>(
                        HttpMethod.Post, url, info);
        }

        public async Task<AccountUploadProfileImageResponse> AccountUploadProfileImagesync(string accountId,
            string imagePath)
        {
            const int bufferSize = 1024*1024;
            using (
                var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true)
                )
            {
                return await this.AccountUploadProfileImagesync(accountId, stream);
            }
        }

        public async Task<AccountUploadProfileImageResponse> AccountUploadProfileImagesync(string accountId,
            Stream imageStream)
        {
            var url = this.GenerateApiEndpoint("account", $"upload/{accountId}");
            var content = new StreamContent(imageStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return
                await
                    httpClient.SendAsyncWithException<HttpContent, AccountUploadProfileImageResponse>(HttpMethod.Post,
                        url, content);
        }

        private string GenerateApiEndpoint(string controller, string route, string queryString = null)
        {
            var url = $"{this.host}/api/{controller}/{route}";
            if (!string.IsNullOrEmpty(queryString))
            {
                url = $"{url}/?{queryString}";
            }

            return url;
        }
    }
}