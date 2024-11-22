using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using MonoBehaviour = Framework.Architecture.MonoBehaviour;
namespace Framework.UI
{
    public class WindowTitlebar : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        [SerializeField]
        protected   TextMeshProUGUI                         windowTitle;
        [SerializeField]
        protected   bool                                    moveable        = false;
        private     Vector2                                 beforeMovedPos  = Vector2.zero;
        private     Transform                               parentWndTrans  = null;
        private     WeakReference<WindowControllerBase>     parentWindow    = new WeakReference<WindowControllerBase>();

        public string WindowTitle
        {
            set { windowTitle.text = value; }
        }

        private void Awake()
        {
            parentWindow.Target = this.GetComponentInParent<WindowControllerBase>();
            parentWindow.Target.SetWindowTitlebar(this);
            parentWndTrans = parentWindow.Target.transform;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if(moveable)
                parentWndTrans.position = beforeMovedPos + (eventData.position - eventData.pressPosition);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            beforeMovedPos = parentWndTrans.position;
        }

        public void OnBtnClickedClose()
        {
            parentWindow.Target.OnKeyUp(KeyCode.Escape, false, false, false);
        }
    }
}