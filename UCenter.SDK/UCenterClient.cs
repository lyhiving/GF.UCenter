using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Models;
using UCenter.Common.Database.Entities;
using AppReadDataResponse = UCenter.SDK.Response.AppReadDataResponse;
using AppWriteDataResponse = UCenter.SDK.Response.AppWriteDataResponse;

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


        public async Task<AccountEntity> AccountRegisterAsync(AccountRegisterInfo info)
        {
            string url = GenerateApiEndpoint("account", "register");
            var response = await httpClient.SendAsyncWithException<AccountRegisterInfo, AccountEntity>(HttpMethod.Post, url, info);
            return response;
        }

        public async Task<AccountEntity> AccountLoginAsync(AccountLoginInfo info)
        {
            string url = GenerateApiEndpoint("account", "login");
            var response = await httpClient.SendAsyncWithException<AccountLoginInfo, AccountEntity>(HttpMethod.Post, url, info);
            return response;
        }

        public async Task<AccountEntity> AccountChangePassword(AccountChangePasswordInfo info)
        {
            string url = this.GenerateApiEndpoint("account", "changepassword");
            return await httpClient.SendAsyncWithException<AccountChangePasswordInfo, AccountEntity>(HttpMethod.Post, url, info);
        }

        public async Task<AppEntity> AppLoginAsync(AppLoginInfo info)
        {
            string url = GenerateApiEndpoint("app", "login");
            var response = await httpClient.SendAsyncWithException<AppLoginInfo, AppEntity>(HttpMethod.Post, url, info);
            return response;
        }

        public async Task<AccountAppInfo> AppVerifyAccountAsync(AccountAppVerificationInfo info)
        {
            string url = GenerateApiEndpoint("app", "verifyaccount");
            var response = await httpClient.SendAsyncWithException<AccountAppVerificationInfo, AccountAppInfo>(HttpMethod.Post, url, info);
            return response;
        }

        //public async Task<AppWriteDataResponse> AppWriteDataAsync()
        //{
        //    string url = $"{this.host}/api/app/writedata";

        //    var appVerifyAccountRequest = new AppWriteDataRequest();
        //    var response = await httpClient.SendAsyncWithException<AppWriteDataResponse>(url, ToHttpContent(appVerifyAccountRequest));
        //    return response;

        //}

        //public async Task<AppReadDataResponse> AppReadDataAsync()
        //{
        //    string url = $"{this.host}/api/app/readdata";
        //    var appVerifyAccountRequest = new AppReadDataResponse();
        //    var response = await httpClient.PostAsync<AppReadDataResponse>(url, ToHttpContent(appVerifyAccountRequest));
        //    return response;
        //}

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
