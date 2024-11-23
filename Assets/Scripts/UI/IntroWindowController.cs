using UnityEngine;
using Framework.UI;

public class IntroWindowController : WindowController<IntroWindow>
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
        MessageBox _msg = MessageBox.Open("로비행?","로비로 갑니다효??", MessageBox.MessageBoxType.MB_OK,
            () =>
            {
                Framework.Scene.SceneManager.Instance.ChangeScene<LobbyScene>();
            }
        );
    }

    protected override bool OnRefreshView()
    {
        if (base.OnRefreshView() == false)
        {
            return false;
        }
        
        IntroWindow _windowModel = this.Model;

        return true;
    }
}

[Framework.Architecture.Window(Path = "Assets/Bundle/ui/Intro/IntroWindow.prefab", Layer = WINDOW_LAYER.MIDDLE)]
public sealed class IntroWindow : Window<IntroWindowController>
{
    public int TextValue { get; set; }
}