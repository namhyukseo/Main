using Framework;
using Framework.Architecture;
using Framework.Scene;
using Framework.Singleton;
using Framework.UI;
using System.Collections.Generic;
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

        //GarbageCollector.GCMode = GarbageCollector.Mode.Manual;
    }

    public void OnUpdate(float _deltaTime)
    {
        List<string> test = new List<string>();
        for(int i=0;i<1000;++i)
        {
            test.Add(string.Format("가나다라 = {0}", i));
        }

        //Debug.LogFormat("수집 {0}", GarbageCollector.CollectIncremental(30000));
    }
}

