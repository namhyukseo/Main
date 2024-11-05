using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.UI;
using UnityEngine.UIElements;

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
        return true;
    }

    public void OnBtnClickedOK()
    {
        UIManager.Instance.OpenCommonWindow<MessageBox>();
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