using UnityEngine;
using System.Collections;
using Eb;
using Ec;

public class EcSampleListener : IEcEngineListener
{
    //-------------------------------------------------------------------------
    public void init(EntityMgr entity_mgr, Entity et_root)
    {
        entity_mgr.regComponent<ClientSampleApp<DefSampleApp>>();
        entity_mgr.regComponent<ClientUCenterSDK<DefUCenterSDK>>();

        entity_mgr.regEntityDef<EtSampleApp>();
        entity_mgr.regEntityDef<EtUCenterSDK>();
    }

    //-------------------------------------------------------------------------
    public void release()
    {
    }
}

public class MbSample : MonoBehaviour
{
    //-------------------------------------------------------------------------
    static MbSample mMbMain;
    EcEngine mEngine;

    //-------------------------------------------------------------------------
    static public MbSample Instance
    {
        get { return mMbMain; }
    }

    //-------------------------------------------------------------------------
    void Awake()
    {
        mMbMain = this;
    }

    //-------------------------------------------------------------------------
    void Start()
    {
        // 初始化系统参数
        {
            Application.runInBackground = true;
            Time.fixedDeltaTime = 0.05f;
            Application.targetFrameRate = 30;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        // 初始化日志
        {
            EbLog.NoteCallback = Debug.Log;
            EbLog.WarningCallback = Debug.LogWarning;
            EbLog.ErrorCallback = Debug.LogError;
        }

        EbLog.Note("MbSample.Start()");

        if (mEngine == null)
        {
            EcEngineSettings settings;
            settings.ProjectName = "EcSample";
            settings.RootEntityType = "EtRoot";
            settings.EnableCoSuperSocket = false;
            mEngine = new EcEngine(ref settings, new EcSampleListener());
        }

        // 创建EtSampleApp
        EntityMgr.Instance.createEntity<EtSampleApp>(null, EcEngine.Instance.EtNode);
    }

    //-------------------------------------------------------------------------
    void Update()
    {
        if (mEngine != null)
        {
            mEngine.update(Time.deltaTime);
        }
    }

    //-------------------------------------------------------------------------
    void OnDestroy()
    {
        _destory();
    }

    //-------------------------------------------------------------------------
    void OnApplicationQuit()
    {
        _destory();
    }

    //-------------------------------------------------------------------------
    void OnApplicationFocus(bool focusStatus)
    {
    }

    //-------------------------------------------------------------------------
    void _destory()
    {
        if (mEngine == null) return;

        if (mEngine != null)
        {
            mEngine.close();
            mEngine = null;
        }

        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        EbLog.Note("MbSample._destory()");
    }
}
