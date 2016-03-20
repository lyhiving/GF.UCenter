using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GF.Common;

public class ClientSampleApp<TDef> : Component<TDef> where TDef : DefSampleApp, new()
{
    //-------------------------------------------------------------------------

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

        AccountLoginInfo login_request = new AccountLoginInfo();
        login_request.AccountName = "test1010";
        login_request.Password = "123456";
        co_ucentersdk.login(login_request, _onUCenterLogin);
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
}
