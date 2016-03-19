using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using GF.Common;

public delegate void OnUCenterRegister(ClientRegisterResponse register_response);
public delegate void OnUCenterLogin(ClientLoginResponse login_response);

public class ClientUCenterSDK<TDef> : Component<TDef> where TDef : DefUCenterSDK, new()
{
    //-------------------------------------------------------------------------
    public string UCenterDomain { get; set; }
    public bool UseSsl { get; set; }
    public WWW WWWRegister { get; private set; }
    public WWW WWWLogin { get; private set; }
    OnUCenterRegister RegisterHandler { get; set; }
    OnUCenterLogin LoginHandler { get; set; }

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
        if (WWWRegister != null)
        {
            if (WWWRegister.isDone)
            {
                ClientRegisterResponse register_response = null;

                if (string.IsNullOrEmpty(WWWRegister.error))
                {
                    try
                    {
                        var ucenter_response = EbTool.jsonDeserialize<UCenterResponse>(WWWLogin.text);
                        register_response = ucenter_response.As<ClientRegisterResponse>();
                        //register_response = EbTool.jsonDeserialize<ClientRegisterResponse>(WWWRegister.text);
                    }
                    catch (Exception ex)
                    {
                        EbLog.Error("ClientUCenterSDK.update() RegisterResponse Error");
                        EbLog.Error(ex.ToString());
                    }
                }

                if (register_response == null)
                {
                    register_response = new ClientRegisterResponse();
                    register_response.result = (short)UCenterResult.Failed;
                }

                if (RegisterHandler != null)
                {
                    RegisterHandler(register_response);
                    RegisterHandler = null;
                }

                WWWRegister = null;
            }
        }

        if (WWWLogin != null)
        {
            if (WWWLogin.isDone)
            {
                ClientLoginResponse login_response = null;

                if (string.IsNullOrEmpty(WWWLogin.error))
                {
                    try
                    {
                        var ucenter_response = EbTool.jsonDeserialize<UCenterResponse>(WWWLogin.text);
                        login_response = ucenter_response.As<ClientLoginResponse>();
                        //login_response = EbTool.jsonDeserialize<ClientLoginResponse>(WWWLogin.text);
                    }
                    catch (Exception ex)
                    {
                        EbLog.Error("ClientUCenterSDK.update() LoginResponse Error");
                        EbLog.Error(ex.ToString());
                    }
                }

                if (login_response == null)
                {
                    login_response = new ClientLoginResponse();
                    login_response.result = (short)UCenterResult.Failed;
                }

                if (LoginHandler != null)
                {
                    LoginHandler(login_response);
                    LoginHandler = null;
                }

                WWWLogin = null;
            }
        }
    }

    //-------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }

    //-------------------------------------------------------------------------
    public void register(ClientRegisterRequest register_request, OnUCenterRegister register_handler)
    {
        if (WWWRegister != null)
        {
            return;
        }

        RegisterHandler = register_handler;

        string http_url = _genUrl("register");

        string param = EbTool.jsonSerialize(register_request);
        byte[] bytes = Encoding.UTF8.GetBytes(param);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Accept"] = "application/json";
        headers["Content-Type"] = "application/json";
        headers["Host"] = UCenterDomain;
        headers["User-Agent"] = "";

        WWWRegister = new WWW(http_url, bytes, headers);
    }

    //-------------------------------------------------------------------------
    public void login(ClientLoginRequest login_request, OnUCenterLogin login_handler)
    {
        if (WWWLogin != null)
        {
            return;
        }

        LoginHandler = login_handler;

        string http_url = _genUrl("login");

        string param = EbTool.jsonSerialize(login_request);
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
    string _genUrl(string api)
    {
        string http_url = null;
        if (UseSsl)
        {
            http_url = string.Format("https://{0}/ucenter/api/account/{1}",
            UCenterDomain, api);
        }
        else
        {
            http_url = string.Format("http://{0}/ucenter/api/account/{1}",
            UCenterDomain, api);
        }

        return http_url;
    }
}
