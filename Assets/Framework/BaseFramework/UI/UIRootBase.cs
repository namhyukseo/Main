using System.Collections.Generic;
using UnityEngine;
using Framework.Architecture;
using UnityEngine.UIElements;


namespace Framework.UI
{

    public abstract class UIRootBase : Architecture.MonoBehaviour, Architecture.MonoBehaviour.IPostLateUpdate, Architecture.MonoBehaviour.IUpdate, Architecture.MonoBehaviour.IOnGUIEvent,
        KeyEventHandler.IKeyDownEvent, KeyEventHandler.IKeyUpEvent
    {
        [SerializeField] private GameObject closeWindowsRoot;
        [SerializeField] private Canvas     rootCanvas;
        readonly private WeakReference<WindowLayer>[] layers = new WeakReference<WindowLayer>[(int)WINDOW_LAYER.MAX_LAYER];

        public static WeakReference<UIRootBase> uiRoot = null;

        public static void Create(ResourceAttribute _attribute)
        {
            ResourceLoader.Instance.LoadGameObject<UIRootBase>(_attribute, null);
        }
        protected virtual void Awake()
        {
            UIRootBase.uiRoot = new WeakReference<UIRootBase>(this);

            DontDestroyOnLoad(this.gameObject);

            foreach (var iter in this.GetComponentsInChildren<WindowLayer>())
            {
                Debug.Assert(layers[(int)iter.Layer] == null);
                layers[(int)iter.Layer] = new WeakReference<WindowLayer>(iter);
            }

            KeyEventHandler.RegistKeyEventFunc(EventType.KeyUp, OnKeyUp);
            KeyEventHandler.RegistKeyEventFunc(EventType.KeyDown, OnKeyDown);
        }
    
        public void Attach(WindowControllerBase _window)
        {
            WeakReference<WindowLayer> _layerRef = layers[(int)_window.Layer];
            if (_layerRef != null)
            {
                _window.transform.SetParent(_layerRef.Target.transform, false);
                _window.AttachedLayer = _layerRef.Target;
            }
        }

        public void Detach(WindowControllerBase _window)
        {
            _window.transform.SetParent(this.closeWindowsRoot.transform, false);
            _window.AttachedLayer = null;
        }


        public bool OnKeyUp(KeyCode _key, bool _alt, bool _ctrl, bool _shift)
        {
            for (int i = (int)WINDOW_LAYER.MAX_LAYER - 1; i >= (int)WINDOW_LAYER.BEGIN; --i)
            {
                WindowLayer _layer = layers[i].Target;
                if (_layer && _layer.OnKeyUp(_key, _alt, _ctrl, _shift))
                {
                    return true;
                }
            }
            //Debug.LogFormat("OnKeyUp={0}, Alt={1}, Ctrl={2}, Shift={3}", _key, _alt, _ctrl, _shift);
            return false;
        }

        public bool OnKeyDown(KeyCode _key, bool _alt, bool _ctrl, bool _shift)
        {
            for (int i = (int)WINDOW_LAYER.MAX_LAYER - 1; i >= (int)WINDOW_LAYER.BEGIN; --i)
            {
                WindowLayer _layer = layers[i].Target;
                if (_layer && _layer.OnKeyDown(_key, _alt, _ctrl, _shift))
                {
                    return true;
                }
            }
            //Debug.LogFormat("OnKeyDown={0}, Alt={1}, Ctrl={2}, Shift={3}", _key, _alt, _ctrl, _shift);
            return false;
        }
        public void OnPostLateUpdate(float _deltaTime)
        {
            for (int i = (int)WINDOW_LAYER.MAX_LAYER - 1; i >= (int)WINDOW_LAYER.BEGIN; --i)
            {
                WindowLayer _layer = layers[i].Target;
                if (_layer)
                {
                    _layer.Sort();
                }
            }

        }
        public void OnUpdate(float _deltaTime)
        {
            
        }
        public void OnGUIEvent(Event _event)
        {
            KeyEventHandler.OnKeyProcess(_event);
        }

        protected void OnDestroy()
        {
            KeyEventHandler.UnregistKeyEventFunc(EventType.KeyDown, OnKeyDown);
            KeyEventHandler.UnregistKeyEventFunc(EventType.KeyUp, OnKeyUp);
        }
    }
}