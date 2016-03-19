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
        co_ucentersdk.UseSsl = true;

        ClientLoginRequest login_request = new ClientLoginRequest();
        login_request.acc = "test1010";
        login_request.pwd = "123456";
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
    void _onUCenterLogin(ClientLoginResponse login_response)
    {
        EbLog.Note("ClientSampleApp._onUCenterLogin() UCenterResult=" + login_response.result);
    }
}
