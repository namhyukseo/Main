using Framework.Scene;

public class LobbySceneController : Framework.Scene.SceneController<LobbyScene>
{
    protected override bool OnRefreshView()
    {
        return true;
    }
}

[Framework.Architecture.Scene(Path = "Assets/Bundle/Scene/LobbyScene.unity")]
public class LobbyScene : Framework.Scene.SceneModel<LobbySceneController>
{
    public override void OnEnterScene(SceneModelBase _beforeActiveScene)
    {
        base.OnEnterScene(_beforeActiveScene);

        Framework.UI.UIManager.Instance.Open<LobbyWindow>();
    }

    public override void OnExitScene(SceneModelBase _nextActiveScene)
    {
        base.OnExitScene(_nextActiveScene);

        Framework.UI.UIManager.Instance.Close<LobbyWindow>();
    }
}