using System;
using UCenter.Common.Models;

namespace UCenter.SDK.Response
{
    [Serializable]
    public class AppWriteDataResponse : UCenterResponse
    {
        public UCenterResult result;
        public string app_id;
        public AppData app_data;
    }
}
