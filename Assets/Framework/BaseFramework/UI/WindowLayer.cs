using System.Collections.Generic;
using UnityEngine;
using MonoBehaviour = Framework.Architecture.MonoBehaviour;

namespace Framework.UI
{
    public enum WINDOW_LAYER : byte
    {
        /// <summary>
        /// Layer가 설정되지 않은 경우의 기본 값.
        /// </summary>
        BEGIN = 0,
        /// <summary>
        /// 제일 하단에 깔리는 UI들이 위치하는 Layer ( World객체의 터치를 감지하거나 조이스틱 등의 UI 컨트롤러가 위치하는? )
        /// </summary>
        BOTTOM_MOST = BEGIN,
        /// <summary>
        /// 하단에 깔려 상시 열려있는 UI들이 위치하는 Layer ( HUD창 등 )
        /// </summary>
        BOTTOM,
        /// <summary>
        /// 기본적인 UI들이 위치하는 Layer ( 인벤토리 창 등 )
        /// </summary>
        MIDDLE,
        /// <summary>
        /// PopupWindow나 Context Menu같은 UI들이 위치하는 Layer
        /// </summary>
        TOP,
        /// <summary>
        /// System Message와 같은 최상위 UI들이 위치하는 Layer
        /// </summary>
        TOP_MOST,
        MAX_LAYER,
    };
    public class WindowLayer : MonoBehaviour, KeyEventHandler.IKeyDownEvent, KeyEventHandler.IKeyUpEvent
    {
        private static int CompareOpenTime(WindowControllerBase _left, WindowControllerBase _right)
        {
            if(_left == null || _right == null) return 0;

            return _left.OpenTime <= _right.OpenTime ? -1 : 1;
        }

        public void SetRefreshDirtyFlag()
        {
            refreshDirtyFlag = true;
        }

        private bool refreshDirtyFlag = false;
        public WINDOW_LAYER Layer { get { return layer; } }

        [SerializeField]    private WINDOW_LAYER            layer;
        [ExposeAttriubte]   private List<WindowControllerBase>   openedWindows = new List<WindowControllerBase>();

        public bool CalcSortingOrder()
        {
            if(refreshDirtyFlag == true)
            {
                int i = 0;
                openedWindows.Sort(CompareOpenTime);
                foreach(WindowControllerBase win in openedWindows) 
                {
                    win.WindowCanvas.overrideSorting = true;
                    win.WindowCanvas.sortingOrder = (int)layer * 10000 + (i*100);
                    ++i;
                }
                refreshDirtyFlag = false;
                
                return true;
            }
            return false;
        }

        public WindowControllerBase GetTopModalWindowController()
        {
            for(int i=openedWindows.Count-1; i>=0; i--) 
            {
                WindowControllerBase win = openedWindows[i];
                if(win.IsModal)
                    return win;
            }
            return null;
        }

        public void OnAttach(WindowControllerBase _window)
        {
            if(openedWindows.Contains(_window))
                return;

            openedWindows.Add(_window);

        }

        public void OnDetach(WindowControllerBase _window)
        {
            openedWindows.Remove(_window);
        }

        public bool OnKeyDown(KeyCode _keyCode, bool _alt, bool _ctrl, bool _shift)
        {
            for(int i=openedWindows.Count-1; i>=0; i--) 
            {
                WindowControllerBase win = openedWindows[i];
                if (win.OnKeyDown(_keyCode, _alt, _ctrl, _shift)) return true;
            }
            return false;
        }

        public bool OnKeyUp(KeyCode _keyCode, bool _alt, bool _ctrl, bool _shift)
        {
            for (int i = openedWindows.Count - 1; i >= 0; i--)
            {
                WindowControllerBase win = openedWindows[i];
                if (win.OnKeyUp(_keyCode, _alt, _ctrl, _shift)) return true;
            }
            return false;
        }
    }
}