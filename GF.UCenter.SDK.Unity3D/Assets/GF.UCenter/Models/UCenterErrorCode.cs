using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    public enum UCenterErrorCode : short
    {
        /// <summary>
        /// 通用，成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 10,

        /// <summary>
        /// 超时
        /// </summary>
        Timeout = 20,

        /// <summary>
        /// 注册，用户名重复
        /// </summary>
        AccountRegisterFailedAlreadyExist = 100,

        /// <summary>
        /// 登陆，帐号不存在
        /// </summary>
        AccountLoginFailedNotExist = 200,

        /// <summary>
        /// 登陆，密码错误
        /// </summary>
        AccountLoginFailedPasswordError,

        /// <summary>
        /// App验证登录，帐号不存在
        /// </summary>
        AppLoginFailedNotExit = 300,

        /// <summary>
        /// App登陆失败，secret错误
        /// </summary>
        AppLoginFailedSecretError,

        /// <summary>
        /// 读取AppData失败
        /// </summary>
        LoginVerifyReadAppDataFailed,

        /// <summary>
        /// 写入AppData失败
        /// </summary>
        LoginVerifyWriteAppDataFailed,

        /// <summary>
        /// 创建Charge失败
        /// </summary>
        CreateChargeFailed = 400,


        CouchBaseError = 600,
    }
}
