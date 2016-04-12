using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using GF.Common;
using GF.UCenter.Common.Portable;

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
        co_ucentersdk.UCenterDomain = "http://cragonucenter.chinacloudsites.cn/";

        // 注册
        AccountRegisterInfo register_request = new AccountRegisterInfo();
        register_request.AccountName = "aaaaabbbb";
        register_request.Password = "123456";
        register_request.SuperPassword = "12345678";
        //co_ucentersdk.register(register_request, _onUCenterRegister);

        // 登录
        AccountLoginInfo login_request = new AccountLoginInfo();
        login_request.AccountName = "aaaaabbbb";
        login_request.Password = "123456";
        co_ucentersdk.login(login_request, _onUCenterLogin);

        // 游客登录
        //co_ucentersdk.guest(_onUCenterGuestLogin);

        // 游客帐号转正
        AccountConvertInfo convert_info = new AccountConvertInfo();
        convert_info.AccountId = "01e94810-ce14-4fff-9c06-16a77990e12c";
        convert_info.AccountName = "asdfg";
        convert_info.OldPassword = "";
        convert_info.Password = "";
        convert_info.SuperPassword = "";
        convert_info.Sex = Sex.Unknown;
        convert_info.Name = "";
        convert_info.IdentityNum = "";
        convert_info.PhoneNum = "";
        convert_info.Email = "";
        //co_ucentersdk.convert(convert_info, _onUCenterConvert);

        // 重置密码
        AccountResetPasswordInfo resetpassword_request = new AccountResetPasswordInfo();
        login_request.AccountName = "aaaaabbbb";
        login_request.Password = "123456";
        //co_ucentersdk.resetPassword(resetpassword_request, _onUCenterResetPassword);

        // 上传图片
        string account_id = "1111";
        byte[] buffer = new byte[100];
        MemoryStream ms = new MemoryStream(buffer);
        co_ucentersdk.uploadProfileImage(account_id, ms, _onUCenterUploadProfileImage);
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
            EbLog.Note("ErrorCode=" + error.ErrorCode);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
    }

    //-------------------------------------------------------------------------
    void _onUCenterLogin(UCenterResponseStatus status, AccountLoginResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterLogin() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.ErrorCode);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
    }

    //-------------------------------------------------------------------------
    void _onUCenterGuestLogin(UCenterResponseStatus status, AccountGuestLoginResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterGuestLogin() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.ErrorCode);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
        else
        {
            EbLog.Note("AccountId=" + response.AccountId);
            EbLog.Note("AccountName=" + response.AccountName);
        }
    }

    //-------------------------------------------------------------------------
    void _onUCenterConvert(UCenterResponseStatus status, AccountConvertResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterConvert() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.ErrorCode);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
        //else
        //{
        //    EbLog.Note("AccountId=" + response.AccountId);
        //    EbLog.Note("AccountName=" + response.AccountName);
        //}
    }

    //-------------------------------------------------------------------------
    void _onUCenterResetPassword(UCenterResponseStatus status, AccountResetPasswordResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterResetPassword() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.ErrorCode);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
    }

    //-------------------------------------------------------------------------
    void _onUCenterUploadProfileImage(UCenterResponseStatus status, AccountUploadProfileImageResponse response, UCenterError error)
    {
        EbLog.Note("ClientSampleApp._onUCenterUploadProfileImage() UCenterResult=" + status);

        if (error != null)
        {
            EbLog.Note("ErrorCode=" + error.ErrorCode);
            EbLog.Note("ErrorMessage=" + error.Message);
        }
    }
}
