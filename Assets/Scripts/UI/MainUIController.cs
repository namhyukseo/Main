/// <summary>
/// A simple controller for switching between UI panels.
/// </summary>
public class MainUIController : Framework.Architecture.MonoBehaviour
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Framework.Scene.SceneManager.Instance.ChangeScene<LobbyScene>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Framework.UI.UIManager.Instance.Close<LobbyWindow>();
    }
}