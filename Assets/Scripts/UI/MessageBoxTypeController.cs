using UnityEngine;
using Framework.UI;
using TMPro;
using System;

public class MessageBoxTypeController : WindowController<MessageBox>
{
    [SerializeField]
    protected   TextMeshProUGUI message;
    [SerializeField]
    protected   GameObject  goOKCancelBtnGroup;
    [SerializeField]
    protected   GameObject  goYesNoBtnGroup;
    [SerializeField]
    protected   GameObject  goOKBtnGroup;

    protected override bool OnRefreshView()
    {
        if (base.OnRefreshView() == false)
            return false;

        MessageBox _windowModel = this.Model;

        this.windowTitlebar.Target.WindowTitle = _windowModel.WindowTitle;
        this.message.text = _windowModel.WindowMessage;

        switch(_windowModel.Type)
        {
            case MessageBox.MessageBoxType.MB_OK:
            {
                goYesNoBtnGroup.SetActive(false);
                goOKCancelBtnGroup.SetActive(false);
                goOKBtnGroup.SetActive(true);
            }
            break;
            case MessageBox.MessageBoxType.MB_OKCANCEL:
            {
                goYesNoBtnGroup.SetActive(false);
                goOKCancelBtnGroup.SetActive(true);
                goOKBtnGroup.SetActive(false);
            }
            break;
            case MessageBox.MessageBoxType.MB_YESNO:
            {
                goYesNoBtnGroup.SetActive(true);
                goOKCancelBtnGroup.SetActive(false);
                goOKBtnGroup.SetActive(false);
            }
            break;
        }

        return true;
    }

    public void OnOK()
    {
        this.Model?.onOK?.Invoke();

        this.Model?.Close();
    }

    public void OnCancel()
    {
        this.Model?.onCancel?.Invoke();

        this.Model?.Close();
    }
}
[Framework.Architecture.Window(Path = "Assets/Bundle/ui/MessageBox/MessageBox.prefab", Layer = WINDOW_LAYER.TOP, IsModal = true)]
public sealed class MessageBox : CommonWindow<MessageBoxTypeController>
{
    public enum MessageBoxType
    {
        MB_OK,
        MB_OKCANCEL,
        MB_YESNO,
    };

    public static MessageBox Open(string _title, string _message, MessageBoxType _type = MessageBoxType.MB_OK, Action _ok = null, Action _cancel = null)
    {
        return UIManager.Instance.OpenCommonWindow<MessageBox>(_title, _message, _type, _ok, _cancel);
    }

    public override void OnOpen(params object[] _parames)
    {
        base.OnOpen(_parames);

        WindowTitle = _parames[0] as string;
        WindowMessage = _parames[1] as string;
        Type = (MessageBoxType)_parames[2];
        onOK = _parames[3] as Action;
        onCancel = _parames[4] as Action;
    }

    public MessageBoxType Type { get; private set; }
    public string WindowTitle { get; set; }
    public string WindowMessage { get; set; }

    public Action onOK{ get; set; }
    public Action onCancel{ get; set; }
}
