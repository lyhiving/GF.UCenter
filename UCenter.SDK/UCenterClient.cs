using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Models;

namespace UCenter.SDK
{
    public class UCenterClient
    {
        private readonly UCenterHttpClient httpClient;
        private readonly string host;

        public UCenterClient(string host)
        {
            this.httpClient = new UCenterHttpClient();
            this.host = host;
        }


        public async Task<AccountRegisterResponse> AccountRegisterAsync(AccountRegisterInfo info)
        {
            string url = $"{this.host}/api/account/register";
            var response = await httpClient.PostAsync<AccountRegisterResponse>(url, ToHttpContent(info));
            return response;
        }

        public async Task<AccountLoginResponse> AccountLoginAsync(AccountLoginInfo info)
        {
            string url = $"{this.host}/api/account/login";
            var response = await httpClient.PostAsync<AccountLoginResponse>(url, ToHttpContent(info));
            return response;
        }

        public async Task<AppLoginResponse> AppLoginAsync(AppLoginInfo info)
        {
            string url = $"{this.host}/api/app/login";
            var response = await httpClient.PostAsync<AppLoginResponse>(url, ToHttpContent(info));
            return response;
        }

        public async Task<AppVerifyAccountResponse> AppVerifyAccountAsync(AppAccountVerificationInfo info)
        {
            string url = $"{this.host}/api/app/verifyaccount";
            var appVerifyAccountRequest = new AppVerifyAccountRequest();
            var response = await httpClient.PostAsync<AppVerifyAccountResponse>(url, ToHttpContent(appVerifyAccountRequest));
            return response;

        }

        public async Task<AppWriteDataResponse> AppWriteDataAsync()
        {
            string url = $"{this.host}/api/app/writedata";

            var appVerifyAccountRequest = new AppWriteDataRequest();
            var response = await httpClient.PostAsync<AppWriteDataResponse>(url, ToHttpContent(appVerifyAccountRequest));
            return response;

        }

        public async Task<AppReadDataResponse> AppReadDataAsync()
        {
            string url = $"{this.host}/api/app/readdata";
            var appVerifyAccountRequest = new AppReadDataResponse();
            var response = await httpClient.PostAsync<AppReadDataResponse>(url, ToHttpContent(appVerifyAccountRequest));
            return response;
        }

        private ObjectContent ToHttpContent<T>(T data)
        {
            return new ObjectContent<T>(data, new JsonMediaTypeFormatter());
        }
    }
}
