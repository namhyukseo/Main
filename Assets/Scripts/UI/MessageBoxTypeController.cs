using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.UI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MessageBoxTypeController : WindowController<MessageBox>
{
    protected override bool OnRefreshView()
    {
        if (base.OnRefreshView() == false)
            return false;

        MessageBox _windowModel = this.Model as MessageBox;
        return true;
    }
}

[Framework.Architecture.Window(Path = "Assets/Bundle/ui/MessageBox/MessageBox.prefab", Layer = WINDOW_LAYER.TOP)]
public sealed class MessageBox : CommonWindow<MessageBoxTypeController>
{
    public int TextValue { get; set; }

}