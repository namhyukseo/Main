using Framework;
using Framework.Architecture;
using Framework.Scene;
using Framework.UI;
using System.Collections;
using System.IO;
using UnityEngine;


public static class Main
{
    [RuntimeInitializeOnLoadMethod]
    public static void Run()
    {
        UIManager.Create();
        SceneManager.Create();
        ResourceLoader.Create();
        ObjectPool.Create();
        RootMonoBehaviour.Create();

        var _attibute = new ResourceAttribute();
        _attibute.Path = "Assets/Bundle/ui/UIRoot.prefab";
        UIRootBase.Create(_attibute);        

        Framework.Scene.SceneManager.Instance.ChangeScene<LobbyScene>();
    }
}

