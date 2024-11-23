using UnityEngine;
using Framework.UI;

public class LobbyWindowController : WindowController<LobbyWindow>
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnOpen()
    {
        base.OnOpen();
    }

    protected override bool OnEscape()
    {
        MessageBox _msg = MessageBox.Open(LocalizedStringID.ID_QUIT_GAME.ToLocalizedString(), LocalizedStringID.ID_QUIT_GAME_MESSAGE.ToLocalizedString(), MessageBox.MessageBoxType.MB_OKCANCEL,
            () => 
            {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                Application.OpenURL(webplayerQuitURL);
        #else
                Application.Quit();
        #endif        
            }
        );

        return true;
    }

    public void OnBtnClickedOK()
    {
        MessageBox _msg = MessageBox.Open("인트로행?","인트로로 갑니다효??", MessageBox.MessageBoxType.MB_OK,
            () =>
            {
                Framework.Scene.SceneManager.Instance.ChangeScene<IntroScene>();
            }
        );
    }

    protected override bool OnRefreshView()
    {
        if (base.OnRefreshView() == false)
        {
            return false;
        }
        
        LobbyWindow _windowModel = this.Model;
        //textField.text = _windowModel.TextValue.ToString();

        return true;
    }
}

[Framework.Architecture.Window(Path = "Assets/Bundle/ui/Lobby/LobbyWindow.prefab", Layer = WINDOW_LAYER.MIDDLE)]
public sealed class LobbyWindow : Window<LobbyWindowController>
{
    public int TextValue { get; set; }
}