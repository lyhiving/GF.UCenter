using System.Net.Http;
using System.Threading.Tasks;
using GF.UCenter.Common;
using GF.UCenter.Common.Portable;

namespace GF.UCenter.SDK.AppServer
{
    public class UCenterClient
    {
        //---------------------------------------------------------------------
        private readonly UCenterHttpClient httpClient;
        private readonly string host;

        //---------------------------------------------------------------------
        public UCenterClient(string host)
        {
            this.httpClient = new UCenterHttpClient();
            this.host = host;
        }

        //---------------------------------------------------------------------
        public async Task<AppResponse> AppCreateAsync(AppInfo info)
        {
            string url = GenerateApiEndpoint("app", "create");
            var response = await httpClient.SendAsyncWithException<AppInfo, AppResponse>(HttpMethod.Post, url, info);
            return response;
        }

        //---------------------------------------------------------------------
        public async Task<AppVerifyAccountResponse> AppVerifyAccountAsync(AppVerifyAccountInfo info)
        {
            string url = GenerateApiEndpoint("app", "verifyaccount");
            var response = await httpClient.SendAsyncWithException<AppVerifyAccountInfo, AppVerifyAccountResponse>(HttpMethod.Post, url, info);
            return response;
        }

        //---------------------------------------------------------------------
        public async Task<AppAccountDataResponse> AppReadAccountDataAsync(AppAccountDataInfo info)
        {
            string url = GenerateApiEndpoint("app", "readdata");
            var response = await httpClient.SendAsyncWithException<AppAccountDataInfo, AppAccountDataResponse>(HttpMethod.Post, url, info);
            return response;
        }

        //---------------------------------------------------------------------
        public async Task<AppAccountDataResponse> AppWriteAccountDataAsync(AppAccountDataInfo info)
        {
            string url = GenerateApiEndpoint("app", "writedata");
            var response = await httpClient.SendAsyncWithException<AppAccountDataInfo, AppAccountDataResponse>(HttpMethod.Post, url, info);
            return response;
        }

        //---------------------------------------------------------------------
        public async Task<Charge> CreateChargeAsync(ChargeInfo info)
        {
            string url = GenerateApiEndpoint("payment", "charge");
            var response = await httpClient.SendAsyncWithException<ChargeInfo, Charge>(HttpMethod.Post, url, info);
            return response;
        }

        //---------------------------------------------------------------------
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
