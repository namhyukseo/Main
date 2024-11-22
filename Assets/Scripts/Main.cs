using Framework;
using Framework.Architecture;
using Framework.Scene;
using Framework.Singleton;
using Framework.UI;
using System.Collections;
using System.IO;
using UnityEngine;


public class Main : Singleton<Main>, IUpdate
{
    [RuntimeInitializeOnLoadMethod]
    public static void Run()
    {
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

    public void OnUpdate(float _deltaTime)
    {
        throw new System.NotImplementedException();
    }
}

