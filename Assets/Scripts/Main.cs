using Framework;
using Framework.Architecture;
using Framework.Scene;
using Framework.Singleton;
using Framework.UI;
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

        logicThreadObject = new ThreadObject(this.OnUpdateForLogicThread, 30);
        logicThreadObject.Start();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        logicThreadObject.Stop();
    }

    public void OnUpdate(float _deltaTime)
    {
    }

    public void OnApplicationQuit()
    {
        SingletonContainer.Release();
    }

    public bool OnUpdateForLogicThread(float _deltaTime)
    {
        return true;
    }

    private ThreadObject logicThreadObject = null;
}

