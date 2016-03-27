using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GF.Common;
using GF.UCenter.Common.Portable;

public delegate void OnUCenterRegister(UCenterResponseStatus status, AccountRegisterResponse response, UCenterError error);
public delegate void OnUCenterLogin(UCenterResponseStatus status, AccountLoginResponse response, UCenterError error);
public delegate void OnUCenterGuestLogin(UCenterResponseStatus status, AccountGuestLoginResponse response, UCenterError error);
public delegate void OnUCenterConvert(UCenterResponseStatus status, AccountConvertResponse response, UCenterError error);
public delegate void OnUCenterResetPassword(UCenterResponseStatus status, AccountResetPasswordResponse response, UCenterError error);
public delegate void OnUCenterUploadProfileImage(UCenterResponseStatus status, AccountUploadProfileImageResponse response, UCenterError error);

public class ClientUCenterSDK<TDef> : Component<TDef> where TDef : DefUCenterSDK, new()
{
    //-------------------------------------------------------------------------
    public string UCenterDomain { get; set; }
    public bool UseSsl { get; set; }
    public WWW WWWRegister { get; private set; }
    public WWW WWWLogin { get; private set; }
    public WWW WWWGuestLogin { get; private set; }
    public WWW WWWConvert { get; private set; }
    public WWW WWWResetPassword { get; private set; }
    public WWW WWWUploadProfileImage { get; private set; }
    Action<UCenterResponseStatus, AccountRegisterResponse, UCenterError> RegisterHandler { get; set; }
    Action<UCenterResponseStatus, AccountLoginResponse, UCenterError> LoginHandler { get; set; }
    Action<UCenterResponseStatus, AccountGuestLoginResponse, UCenterError> GuestLoginHandler { get; set; }
    Action<UCenterResponseStatus, AccountConvertResponse, UCenterError> ConvertHandler { get; set; }
    Action<UCenterResponseStatus, AccountResetPasswordResponse, UCenterError> ResetPasswordHandler { get; set; }
    Action<UCenterResponseStatus, AccountUploadProfileImageResponse, UCenterError> UploadProfileImageHandler { get; set; }

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

        if (_checkResponse<AccountConvertResponse>(WWWConvert, ConvertHandler))
        {
            WWWConvert = null;
            ConvertHandler = null;
        }

        if (_checkResponse<AccountResetPasswordResponse>(WWWResetPassword, ResetPasswordHandler))
        {
            WWWResetPassword = null;
            ResetPasswordHandler = null;
        }

        if (_checkResponse<AccountUploadProfileImageResponse>(WWWUploadProfileImage, UploadProfileImageHandler))
        {
            WWWUploadProfileImage = null;
            UploadProfileImageHandler = null;
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

        Dictionary<string, string> headers = _genHeader(bytes.Length);

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

        Dictionary<string, string> headers = _genHeader(bytes.Length);

        WWWLogin = new WWW(http_url, bytes, headers);
    }

    //-------------------------------------------------------------------------
    public void guest(OnUCenterGuestLogin handler)
    {
        if (WWWGuestLogin != null)
        {
            return;
        }

        GuestLoginHandler = new Action<UCenterResponseStatus, AccountGuestLoginResponse, UCenterError>(handler);

        string http_url = _genUrl("guest");

        WWWForm form = new WWWForm();
        form.AddField("Accept", "application/x-www-form-urlencoded");
        form.AddField("Content-Type", "application/json; charset=utf-8");
        form.AddField("Content-Length", 0);
        form.AddField("Host", UCenterDomain);
        form.AddField("User-Agent", "");

        WWWGuestLogin = new WWW(http_url, form);
    }

    //-------------------------------------------------------------------------
    public void convert(AccountConvertInfo request, OnUCenterConvert handler)
    {
        if (WWWConvert != null)
        {
            return;
        }

        ConvertHandler = new Action<UCenterResponseStatus, AccountConvertResponse, UCenterError>(handler);

        string http_url = _genUrl("convert");

        string param = EbTool.jsonSerialize(request);
        byte[] bytes = Encoding.UTF8.GetBytes(param);

        Dictionary<string, string> headers = _genHeader(bytes.Length);

        WWWConvert = new WWW(http_url, bytes, headers);
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

        Dictionary<string, string> headers = _genHeader(bytes.Length);

        WWWResetPassword = new WWW(http_url, bytes, headers);
    }

    //-------------------------------------------------------------------------
    public void uploadProfileImage(string account_id, MemoryStream stream, OnUCenterUploadProfileImage handler)
    {
        if (WWWUploadProfileImage != null)
        {
            return;
        }

        UploadProfileImageHandler = new Action<UCenterResponseStatus, AccountUploadProfileImageResponse, UCenterError>(handler);

        string http_url = _genUrl("upload/" + account_id);

        byte[] bytes = stream.ToArray();

        Dictionary<string, string> headers = _genHeader(bytes.Length);

        WWWUploadProfileImage = new WWW(http_url, bytes, headers);
    }

    //-------------------------------------------------------------------------
    Dictionary<string, string> _genHeader(int content_len)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Accept"] = "application/x-www-form-urlencoded";
        headers["Content-Type"] = "application/json; charset=utf-8";
        headers["Content-Length"] = content_len.ToString();
        headers["Host"] = UCenterDomain;
        headers["User-Agent"] = "";

        return headers;
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
                        response = EbTool.jsonDeserialize<UCenterResponse>(www.text);
                    }
                    catch (Exception ex)
                    {

                        EbLog.Error("ClientUCenterSDK.update() UCenterResponse Error");
                        EbLog.Error(ex.ToString());
                    }
                }

                www = null;

                if (handler != null)
                {
                    if (response != null)
                    {
                        handler(response.Status, response.As<TResponse>(), response.Error);
                    }
                    else
                    {
                        var error = new UCenterError();
                        error.ErrorCode = UCenterErrorCode.Failed;
                        error.Message = "";
                        handler(UCenterResponseStatus.Error, default(TResponse), error);
                    }

                    handler = null;
                }

                return true;
            }
        }

        return false;
    }
}
