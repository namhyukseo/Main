using Framework.Scene;

public class IntroSceneController : Framework.Scene.SceneController<IntroScene>
{
    protected override bool OnRefreshView()
    {
        return true;
    }
}

[Framework.Architecture.Scene(Path = "Assets/Bundle/Scene/IntroScene.unity")]
public class IntroScene : Framework.Scene.SceneModel<IntroSceneController>
{
    public override void OnEnterScene(SceneModelBase _beforeActiveScene)
    {
        base.OnEnterScene(_beforeActiveScene);

        Framework.UI.UIManager.Instance.Open<IntroWindow>();
    }

    public override void OnExitScene(SceneModelBase _nextActiveScene)
    {
        base.OnExitScene(_nextActiveScene);

        Framework.UI.UIManager.Instance.Close<IntroWindow>();
    }
}