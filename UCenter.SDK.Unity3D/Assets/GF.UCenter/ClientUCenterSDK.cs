using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using GF.Common;

public delegate void OnUCenterRegister(UCenterResponseStatus status, AccountRegisterResponse response, UCenterError error);
public delegate void OnUCenterLogin(UCenterResponseStatus status, AccountLoginResponse response, UCenterError error);
public delegate void OnUCenterGuestLogin(UCenterResponseStatus status, AccountGuestLoginResponse response, UCenterError error);
public delegate void OnUCenterResetPassword(UCenterResponseStatus status, AccountResetPasswordResponse response, UCenterError error);

public class ClientUCenterSDK<TDef> : Component<TDef> where TDef : DefUCenterSDK, new()
{
    //-------------------------------------------------------------------------
    public string UCenterDomain { get; set; }
    public bool UseSsl { get; set; }
    public WWW WWWRegister { get; private set; }
    public WWW WWWLogin { get; private set; }
    public WWW WWWGuestLogin { get; private set; }
    public WWW WWWResetPassword { get; private set; }
    Action<UCenterResponseStatus, AccountRegisterResponse, UCenterError> RegisterHandler { get; set; }
    Action<UCenterResponseStatus, AccountLoginResponse, UCenterError> LoginHandler { get; set; }
    Action<UCenterResponseStatus, AccountGuestLoginResponse, UCenterError> GuestLoginHandler { get; set; }
    Action<UCenterResponseStatus, AccountResetPasswordResponse, UCenterError> ResetPasswordHandler { get; set; }

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientUCenterSDK.init()");
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("ClientUCenterSDK.release()");
    }

    //-------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
        if (_checkResponse<AccountRegisterResponse>(WWWRegister, RegisterHandler))
        {
            WWWRegister = null;
            RegisterHandler = null;
        }

        if (_checkResponse<AccountLoginResponse>(WWWLogin, LoginHandler))
        {
            WWWLogin = null;
            LoginHandler = null;
        }

        if (_checkResponse<AccountGuestLoginResponse>(WWWGuestLogin, GuestLoginHandler))
        {
            WWWGuestLogin = null;
            GuestLoginHandler = null;
        }

        if (_checkResponse<AccountResetPasswordResponse>(WWWResetPassword, ResetPasswordHandler))
        {
            WWWResetPassword = null;
            ResetPasswordHandler = null;
        }
    }

    //-------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }

    //-------------------------------------------------------------------------
    public void register(AccountRegisterInfo request, OnUCenterRegister handler)
    {
        if (WWWRegister != null)
        {
            return;
        }

        RegisterHandler = new Action<UCenterResponseStatus, AccountRegisterResponse, UCenterError>(handler);

        string http_url = _genUrl("register");

        string param = EbTool.jsonSerialize(request);
        byte[] bytes = Encoding.UTF8.GetBytes(param);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Accept"] = "application/json";
        headers["Content-Type"] = "application/json";
        headers["Host"] = UCenterDomain;
        headers["User-Agent"] = "";

        WWWRegister = new WWW(http_url, bytes, headers);
    }

    //-------------------------------------------------------------------------
    public void login(AccountLoginInfo request, OnUCenterLogin handler)
    {
        if (WWWLogin != null)
        {
            return;
        }

        LoginHandler = new Action<UCenterResponseStatus, AccountLoginResponse, UCenterError>(handler);

        string http_url = _genUrl("login");

        string param = EbTool.jsonSerialize(request);
        byte[] bytes = Encoding.UTF8.GetBytes(param);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Accept"] = "application/json";
        headers["Content-Type"] = "application/json";
        headers["Host"] = UCenterDomain;
        headers["User-Agent"] = "";
        //headers["Connection"] = "Keep-Alive";

        WWWLogin = new WWW(http_url, bytes, headers);
    }

    //-------------------------------------------------------------------------
    public void guestLogin(OnUCenterGuestLogin handler)
    {
        if (WWWGuestLogin != null)
        {
            return;
        }

        GuestLoginHandler = new Action<UCenterResponseStatus, AccountGuestLoginResponse, UCenterError>(handler);

        string http_url = _genUrl("guestlogin");

        //string param = EbTool.jsonSerialize(login_request);
        //byte[] bytes = Encoding.UTF8.GetBytes(param);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Accept"] = "application/json";
        headers["Content-Type"] = "application/json";
        headers["Host"] = UCenterDomain;
        headers["User-Agent"] = "";
        //headers["Connection"] = "Keep-Alive";

        WWWGuestLogin = new WWW(http_url, null, headers);
    }

    //-------------------------------------------------------------------------
    public void resetPassword(AccountResetPasswordInfo request, OnUCenterResetPassword handler)
    {
        if (WWWResetPassword != null)
        {
            return;
        }

        ResetPasswordHandler = new Action<UCenterResponseStatus, AccountResetPasswordResponse, UCenterError>(handler);

        string http_url = _genUrl("resetpassword");

        string param = EbTool.jsonSerialize(request);
        byte[] bytes = Encoding.UTF8.GetBytes(param);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Accept"] = "application/json";
        headers["Content-Type"] = "application/json";
        headers["Host"] = UCenterDomain;
        headers["User-Agent"] = "";
        //headers["Connection"] = "Keep-Alive";

        WWWResetPassword = new WWW(http_url, bytes, headers);
    }

    //-------------------------------------------------------------------------
    string _genUrl(string api)
    {
        string http_url = null;
        if (UseSsl)
        {
            http_url = string.Format("https://{0}/api/account/{1}",
            UCenterDomain, api);
        }
        else
        {
            http_url = string.Format("http://{0}/api/account/{1}",
            UCenterDomain, api);
        }

        return http_url;
    }

    //-------------------------------------------------------------------------
    bool _checkResponse<TResponse>(WWW www, Action<UCenterResponseStatus, TResponse, UCenterError> handler)
    {
        if (www != null)
        {
            if (www.isDone)
            {
                UCenterResponse response = null;

                if (string.IsNullOrEmpty(www.error))
                {
                    try
                    {
                        response = EbTool.jsonDeserialize<UCenterResponse>(WWWLogin.text);
                    }
                    catch (Exception ex)
                    {
                        EbLog.Error("ClientUCenterSDK.update() UCenterResponse Error");
                        EbLog.Error(ex.ToString());
                    }
                }

                if (handler != null)
                {
                    if (response != null)
                    {
                        handler(response.status, response.As<TResponse>(), response.error);
                    }
                    else
                    {
                        handler(UCenterResponseStatus.Error, default(TResponse), null);
                    }

                    handler = null;
                }

                www = null;

                return true;
            }
        }

        return false;
    }
}
