using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    public enum UCenterResult : short
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
        RegisterAccountExist = 100,

        /// <summary>
        /// 登陆，帐号不存在
        /// </summary>
        LoginAccountNotExist = 200,

        /// <summary>
        /// 登陆，密码错误
        /// </summary>
        LoginPwdError,

        /// <summary>
        /// App验证登录，帐号不存在
        /// </summary>
        LoginVerifyAccountNotExit = 300,

        /// <summary>
        /// 非法App
        /// </summary>
        LoginVerifyInvalidApp,

        /// <summary>
        /// 读取AppData失败
        /// </summary>
        LoginVerifyReadAppDataFailed,

        /// <summary>
        /// 写入AppData失败
        /// </summary>
        LoginVerifyWriteAppDataFailed,
    }
}
