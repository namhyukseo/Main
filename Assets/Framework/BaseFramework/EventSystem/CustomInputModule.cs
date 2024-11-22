using System;
using System.Collections;
using Framework.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Architecture
{
    public class CustomInputModule : UnityEngine.EventSystems.StandaloneInputModule
    {
        private bool ShouldIgnoreEventsOnNoFocus()
        {
            return !EditorApplication.isRemoteConnected;
        }
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


