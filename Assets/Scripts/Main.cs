using System.Threading;
using Framework;
using Framework.Architecture;
using Framework.Scene;
using Framework.Singleton;
using Framework.UI;
using UnityEngine;


public class Main : Framework.Singleton.Singleton<Main>, IUpdate
{
    [RuntimeInitializeOnLoadMethod]
    public static void Run()
    {
        Main.Create();
        UIManager.Create();
        SceneManager.Create();
        ResourceLoader.Create();
        ObjectPool.Create();
        RootMonoBehaviour.Create(Main.Instance.OnApplicationQuit);
        LocalizeStringTable<LocalizedStringID>.Create();
        
        var _attibute = new ResourceAttribute();
        _attibute.Path = "Assets/Bundle/ui/UIRoot.prefab";
        UIRootBase.Create(_attibute);        

        Framework.Scene.SceneManager.Instance.ChangeScene<LobbyScene>();
    }

    protected override void OnInit()
    {
        base.OnInit();

        logicThreadObject = new ThreadObject(this.OnUpdateForLogicThread, 60, "LogicThread");
        logicThreadObject.Start();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        logicThreadObject.Stop();
    }

    public void OnUpdate(float _deltaTime)
    {
        DebugLogForThread.PrintDebugLog();
    }

    public void OnApplicationQuit()
    {
        SingletonContainer.Release();
    }

    System.Random   random = new System.Random((int)Time.time);
    public bool OnUpdateForLogicThread(float _deltaTime)
    {
        int _sleep = random.Next(0, 33);
        DebugLogForThread.LogFormat("[Update ms = {0}, deltaTime = {1}", _sleep, _deltaTime);
        Thread.Sleep(_sleep);
        return true;
    }

    private ThreadObject logicThreadObject = null;
}

