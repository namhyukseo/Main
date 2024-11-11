using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.UI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using System;

public class MessageBoxTypeController : WindowController<MessageBox>
{
    [SerializeField]
    protected   TextMeshProUGUI windowTitle;
    [SerializeField]
    protected   TextMeshProUGUI message;

    protected override bool OnRefreshView()
    {
        if (base.OnRefreshView() == false)
            return false;

        MessageBox _windowModel = this.Model;

        this.windowTitle.text = _windowModel.windowTitle;
        this.message.text = _windowModel.windowMessage;

        return true;
    }

    public void OnOK()
    {
        this.Model.onOK?.Invoke();
    }

    public void OnCancel()
    {
        this.Model.onCancel?.Invoke();
    }
}

[Framework.Architecture.Window(Path = "Assets/Bundle/ui/MessageBox/MessageBox.prefab", Layer = WINDOW_LAYER.TOP, IsModal = true)]
public sealed class MessageBox : CommonWindow<MessageBoxTypeController>
{
    public override void OnOpen(params object[] _parames)
    {
        base.OnOpen(_parames);

        windowTitle = _parames[0] as string;
        windowMessage = _parames[1] as string;
    }

    public string windowTitle { get; set; }
    public string windowMessage { get; set; }

    public Action onOK{ get; set; }
    public Action onCancel{ get; set; }
}
