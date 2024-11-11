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
        MessageBox _msg = UIManager.Instance.OpenCommonWindow<MessageBox>("게임 종료?", "게임을 종료합니까?");

        _msg.onOK = () => UIManager.Instance.OpenCommonWindow<MessageBox>("죵료", "종료합니다.");
        _msg.onCancel = () => _msg.Close();

        return true;
    }

    public void OnBtnClickedOK()
    {
        UIManager.Instance.OpenCommonWindow<MessageBox>("테스트라지요","안녕하세요~ 나며기에요");
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