using Framework.UI;
using UnityEngine.EventSystems;

namespace Framework.Architecture
{
    public class CustomInputModule : UnityEngine.EventSystems.StandaloneInputModule
    {
        public override void Process()
        {
            base.Process();

            PointerEventData _outData = this.GetLastPointerEventData(kMouseLeftId);
            if(_outData != null && _outData.pointerPress != null)
            {
                WindowControllerBase _parentWnd = _outData.pointerPress.GetComponentInParent<WindowControllerBase>();
                _parentWnd.SetFocus();
            }
        }
    }
}


