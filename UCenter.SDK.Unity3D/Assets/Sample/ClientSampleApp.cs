using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GF.Common;
using UCenter.Common.Portable;

public class ClientSampleApp<TDef> : Component<TDef> where TDef : DefSampleApp, new()
{
    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientSampleApp.init()");

        EntityMgr.getDefaultEventPublisher().addHandler(Entity);

        // EtUCenterSDK示例
        var et_ucentersdk = EntityMgr.createEntity<EtUCenterSDK>(null, Entity);
        var co_ucentersdk = et_ucentersdk.getComponent<ClientUCenterSDK<DefUCenterSDK>>();
        co_ucentersdk.UCenterDomain = "cragonucenter.chinacloudsites.cn";
        co_ucentersdk.UseSsl = false;

        // 注册
        AccountRegisterInfo register_request = new AccountRegisterInfo();
        register_request.AccountName = "aaaaabbbb";
        register_request.Password = "123456";
        register_request.SuperPassword = "12345678";
        co_ucentersdk.register(register_request, _onUCenterRegister);

        // 登录
        AccountLoginInfo login_request = new AccountLoginInfo();
        login_request.AccountName = "test1010";
        login_request.Password = "123456";
        co_ucentersdk.login(login_request, _onUCenterLogin);

        // 游客登录
        co_ucentersdk.guestLogin(_onUCenterGuestLogin);

        // 重置密码
        AccountResetPasswordInfo resetpassword_request = new AccountResetPasswordInfo();
        login_request.AccountName = "test1010";
        login_request.Password = "123456";
        co_ucentersdk.resetPassword(resetpassword_request, _onUCenterResetPassword);
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("ClientSampleApp.release()");
    }

    //-------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
    }

    //-------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }

    //-------------------------------------------------------------------------
    void _onUCenterRegister(UCenterResponseStatus status, AccountRegisterResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterRegister() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.Code);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
    }

    //-------------------------------------------------------------------------
    void _onUCenterLogin(UCenterResponseStatus status, AccountLoginResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterLogin() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.Code);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
    }

    //-------------------------------------------------------------------------
    void _onUCenterGuestLogin(UCenterResponseStatus status, AccountGuestLoginResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterGuestLogin() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.Code);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
    }

    //-------------------------------------------------------------------------
    void _onUCenterResetPassword(UCenterResponseStatus status, AccountResetPasswordResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterResetPassword() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.Code);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
    }
}
