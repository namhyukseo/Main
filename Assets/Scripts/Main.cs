using Framework;
using Framework.Architecture;
using Framework.Scene;
using Framework.Singleton;
using Framework.UI;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using UnityEngine;


public class Main : Singleton<Main>, IUpdate
{
    [RuntimeInitializeOnLoadMethod]
    public static void Run()
    {
        Main.Create();
        UIManager.Create();
        SceneManager.Create();
        ResourceLoader.Create();
        ObjectPool.Create();
        RootMonoBehaviour.Create();
        LocalizeStringTable<LocalizedStringID>.Create();
        
        var _attibute = new ResourceAttribute();
        _attibute.Path = "Assets/Bundle/ui/UIRoot.prefab";
        UIRootBase.Create(_attibute);        

        Framework.Scene.SceneManager.Instance.ChangeScene<LobbyScene>();
    }

    protected override void OnInit()
    {
        base.OnInit();

        logicThread = new Thread(this.OnUpdateForLogicThread);
        logicThread.Start();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        stopThread = true;
        if(logicThread.Join(1000) == false ) { logicThread.Abort(); }
    }

    public void OnUpdate(float _deltaTime)
    {
    }

    public void OnUpdateForLogicThread()
    {
        try
        {
            while (!stopThread)
            {
                UIManager.Instance.OnUpdate();
            }
        }
        catch (ThreadAbortException _error)
        {
            Debug.LogException( _error );
        }
    }

    private Thread logicThread = null;
    private bool stopThread = false;
}

