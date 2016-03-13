using System;
using UCenter.Common.Models;

namespace UCenter.SDK.Response
{
    [Serializable]
    public class AppReadDataResponse : UCenterResponse
    {
        public UCenterResult result;
        public string app_id;
        public ulong acc_id;
        public AppDataEntity app_data;
    }

}
