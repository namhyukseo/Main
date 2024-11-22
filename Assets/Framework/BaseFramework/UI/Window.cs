using Framework.Architecture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Framework.UI
{
    public enum WM_MESSAGE : byte
    {
        WM_CLOSE = 0,
        WM_OPEN,
        WM_CREATE,
        WM_DESTROY,
        WM_UPDATE,
        WM_SETFOCUS,
        WM_RELEASEFOCUS,
        WM_MAX = 64, //  WM_MESSAGE�� INT_64�� �����Ǳ⿡ �� 64���� Bitflag ( 0 ~ 63 )�� �ο��ؼ� �����.
    }

    public class WindowMessage
    {
        public WindowMessage(Int64 _flag = 0)
        {
            flag = _flag;
        }
        public void Reset()
        {
            flag = 0;
        }
        public Int64 Flag { get { return flag; } }

        public void SetFlag(WM_MESSAGE _msg)
        {
            flag |= (Int64)(1 << (byte)_msg);
        }

        public void UnsetFlag(WM_MESSAGE _msg)
        {
            flag &= ~(Int64)(1 << (byte)_msg);
        }
        public bool HasFlag(WM_MESSAGE _msg)
        {
            return HasFlag(this.flag, _msg);
        }
        public bool IsEmpty()
        {
            return flag == 0;
        }
        static public bool HasFlag(Int64 _flag, WM_MESSAGE _msg)
        {
            return (_flag & (Int64)(1 << (byte)_msg)) != 0;
        }

        private Int64 flag;
    }

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class WindowControllerBase : Architecture.IController, KeyEventHandler.IKeyDownEvent, KeyEventHandler.IKeyUpEvent, IPointerDownHandler
    {
        static private WeakReference<WindowControllerBase> currentFocusWindow = new WeakReference<WindowControllerBase>();
        private     WeakReference<Canvas> canvas = new WeakReference<Canvas>();
        private     WeakReference<WindowLayer> attachedLayer = new WeakReference<WindowLayer>();
        public      WindowMessage Message { get { return message; } }
        private     WindowMessage message = new WindowMessage();
        public WindowLayer AttachedLayer
        {
            get
            {
                return attachedLayer.Target;
            }
            set
            {
                WindowLayer _layer = attachedLayer.Target;
                if (_layer != null && _layer != value)
                {
                    _layer.OnDetach(this);
                    attachedLayer.Target = null;

                    _layer.SetRefreshDirtyFlag();
                }

                if (value != null)
                {
                    value.OnAttach(this);
                    attachedLayer.Target = value;

                    value.SetRefreshDirtyFlag();
                }
            }
        }
        protected   WeakReference<WindowTitlebar>  windowTitlebar = null;
        public  void SetWindowTitlebar(WindowTitlebar _titleBar)
        {
            windowTitlebar = new WeakReference<WindowTitlebar>(_titleBar);
        }
        
        public Canvas WindowCanvas
        {
            get { return this.canvas.Target; }
        }

        protected override void Awake()
        {
            this.canvas.Target = this.GetComponent<Canvas>();
        }
        protected override void Start()
        {
            this.WindowCanvas.overrideSorting = true;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }   
        protected virtual void OnSetFocus(WindowControllerBase _releaseFocusWindow)
        {
            Debug.LogFormat("[SetFocus] {0}, Relese = {1}", this.ToString(), _releaseFocusWindow ? _releaseFocusWindow.ToString() : "None");
        }
        protected virtual void OnReleaseFocus(WindowControllerBase _setFocusWindow)
        {
            Debug.LogFormat("[ReleseFocus] {0}, SetFocus = {1}", this.ToString(), _setFocusWindow ? _setFocusWindow.ToString() : "None");
        }
        protected virtual void OnCreate()
        {

        }
        protected virtual void OnClose()
        {
            AttachedLayer = null;
        }
        protected virtual void OnOpen() { }
        protected virtual void OnDestroy()
        {
            this.Dispose();
        }

        protected bool WndMsgProc()
        {
            WindowModelBase _window = this.GetModel() as WindowModelBase;

            if (_window == null)
            {
                GameObject.Destroy(this.gameObject);
                return false;
            }

            Int64 _flag = this.Message.Flag;

            if (_flag == 0)
                return false;

            this.Message.Reset();

            if (WindowMessage.HasFlag(_flag, WM_MESSAGE.WM_CLOSE))
            {
                UIRootBase _root = UIRootBase.uiRoot.Target;
                if (_root != null)
                {
                    _root.Detach(this);
                }

                this.SetActive(false);
                this.OnClose();
            }
            else if (WindowMessage.HasFlag(_flag, WM_MESSAGE.WM_OPEN))
            {
                UIRootBase _root = UIRootBase.uiRoot.Target;
                if (_root != null)
                {
                    _root.Attach(this);
                }

                this.SetActive(true);
                this.SetFocus();
                this.OnOpen();
            }

            if (WindowMessage.HasFlag(_flag, WM_MESSAGE.WM_SETFOCUS))
            {
                if (WindowControllerBase.currentFocusWindow.Target == this)
                    return true;

                WindowLayer _layer = this.AttachedLayer;
                if (_layer != null)
                {
                    _layer.SetRefreshDirtyFlag();
                }
                this.OpenTime = Time.time;
                this.OnSetFocus(WindowControllerBase.currentFocusWindow.Target);

                if(WindowControllerBase.currentFocusWindow.Target != null)
                {
                    WindowControllerBase.currentFocusWindow.Target.message.SetFlag(WM_MESSAGE.WM_RELEASEFOCUS);
                }
                WindowControllerBase.currentFocusWindow.Target = this;
            }
            else if (WindowMessage.HasFlag(_flag, WM_MESSAGE.WM_RELEASEFOCUS))
            {
                this.OnReleaseFocus(WindowControllerBase.currentFocusWindow.Target);
            }

            if (WindowMessage.HasFlag(_flag, WM_MESSAGE.WM_DESTROY))
            {
                GameObject.Destroy(this.gameObject);
            }
            else if (WindowMessage.HasFlag(_flag, WM_MESSAGE.WM_CREATE))
            {
                this.OnCreate();
            }

            return true;
        }
        protected override bool OnRefreshView()
        {
            return WndMsgProc();
        }

        public virtual bool OnKeyDown(KeyCode _keyCode, bool _alt, bool _ctrl, bool _shift)
        {
            return false;
        }

        public virtual bool OnKeyUp(KeyCode _keyCode, bool _alt, bool _ctrl, bool _shift)
        {
            if (_keyCode == KeyCode.Escape) { return this.OnEscape(); }
            return false;
        }

        protected abstract bool OnEscape();

        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        public void SetFocus()
        {
            this.Message.SetFlag(WM_MESSAGE.WM_SETFOCUS);
            this.Message.UnsetFlag(WM_MESSAGE.WM_RELEASEFOCUS);
        }

        public float OpenTime
        {
            get
            {
                WindowModelBase _model = this.GetModel() as WindowModelBase;
                if (_model != null)
                {
                    return _model.OpenTime;
                }
                else
                {
                    return 0.0f;
                }
            }
            protected set
            {
                WindowModelBase _model = this.GetModel() as WindowModelBase;
                if (_model != null)
                {
                    _model.OpenTime = value;
                }
            }
        }
        public WINDOW_LAYER Layer
        {
            get
            {
                WindowModelBase _model = this.GetModel() as WindowModelBase;
                if (_model != null)
                {
                    return _model.Layer;
                }
                else
                {
                    return WINDOW_LAYER.MAX_LAYER;
                }
            }
        }
        public bool IsModal
        {
            get
            {
                WindowModelBase _model = this.GetModel() as WindowModelBase;
                if (_model != null)
                {
                    return _model.IsModal;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    /// <summary>
    /// Window�� Controller ( Canvas, Image, Tween�� view��ü�� ��Ʈ�� �ϱ� ���� ��ü )
    /// </summary>
    /// <typeparam name="TModel">Window�𵨰�üŸ��</typeparam>
    public abstract class WindowController<TModel> : WindowControllerBase
        where TModel : WindowModelBase
    {
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
            TModel _model = this.GetModel() as TModel;

            if (_model == null)
            {
                Debug.LogWarningFormat("[{0}]�� [{1}]�� ����Ǿ�� �մϴ�. Ȯ�� ��Ź�帳�ϴ�.", typeof(TModel).ToString(), this.GetType().ToString());
                GameObject.Destroy(this.gameObject);
                return;
            }
            this.gameObject.SetActive(_model.IsActive);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
        protected override void OnCreate()
        {
            base.OnCreate();

            this.transform.localPosition = Vector3.zero;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override bool OnEscape()
        {
            this.Model.Close();
            return true;
        }
        protected TModel Model { get { return this.GetModel() as TModel; } }
    }

    public abstract class WindowModelBase : Architecture.IModel
    {
        private readonly WindowAttribute attribute = null;
        public bool ReservedDestroy { get; set; }
        public bool IsActive { get; private set; }
        public float OpenTime { get; set; }
        public WINDOW_LAYER Layer { get { return this.attribute.Layer; } }
        public bool IsModal { get { return this.attribute.IsModal; } }

        public WindowModelBase()
        {
            this.attribute = WindowAttribute.GetAttribute(this.GetType());
        }
        public static T CreateWindow<T>() where T : WindowModelBase
        {
            var _ret = typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null) as T;
            ResourceLoader.Instance.LoadControllerAsync<T>(_ret.attribute, new WeakReference<T>(_ret));
            return _ret;
        }
        public virtual void OnInit()
        {
            WindowControllerBase _controller = this.Controller as WindowControllerBase;
            if (_controller != null)
            {
                _controller.Message.SetFlag(WM_MESSAGE.WM_CREATE);
                _controller.Message.UnsetFlag(WM_MESSAGE.WM_DESTROY);
            }
        }
        public virtual void OnOpen(params object[] _parames)
        {
            OpenTime = Time.time;

            WindowControllerBase _controller = this.Controller as WindowControllerBase;
            if (_controller != null)
            {
                _controller.Message.SetFlag(WM_MESSAGE.WM_OPEN);
                _controller.Message.UnsetFlag(WM_MESSAGE.WM_CLOSE);
                _controller.gameObject.SetActive(true);
            }
            this.SetActive(true);
        }
        public virtual void OnClose()
        {
            OpenTime = 0;

            WindowControllerBase _controller = this.Controller as WindowControllerBase;
            if (_controller != null)
            {
                _controller.Message.SetFlag(WM_MESSAGE.WM_CLOSE);
                _controller.Message.UnsetFlag(WM_MESSAGE.WM_OPEN);
            }
            this.SetActive(false);
        }
        public virtual void OnDestroy()
        {
            WindowControllerBase _controller = this.Controller as WindowControllerBase;
            if (_controller != null)
            {
                _controller.Message.SetFlag(WM_MESSAGE.WM_DESTROY);
                _controller.Message.UnsetFlag(WM_MESSAGE.WM_CREATE);
            }
        }
        public void SetActive(bool _active)
        {
            this.IsActive = _active;
        }
        public virtual void Close(bool _destroy = false)
        {
            UIManager.Instance.Close(this, _destroy);
        }

        /// <summary>
        /// Model�� ������ Controller�� ���� View�� �˸�
        /// </summary>
        public override void UpdateData()
        {
            IController _controller = this.Controller;
            if (_controller)
            {
                _controller.RefreshDirtyFlag = true;
            }
        }
    }

    public class CommonWindowBase : WindowModelBase, iPoolObject
    {
        public virtual void OnLoad()
        {

        }

        public virtual void OnUnload()
        {

        }
    }

    public class WindowBase : WindowModelBase
    {

    }

    /// <summary>
    /// Window�� Contents Logic�� Data�� ������ �ִ� Model ��ü
    /// </summary>
    public abstract class Window<TController> : WindowBase
        where TController : WindowControllerBase
    {
        public override void SetController(Architecture.IController _controller)
        {
            if (!_controller is TController)
            {
                Debug.LogErrorFormat("Controller type must be {0}!! arg = {1}", typeof(TController), _controller.GetType());
                return;
            }
            base.SetController(_controller);
        }
        public TController GetController()
        {
            return this.Controller as TController;
        }
    }

    /// <summary>
    /// CommonWindow�� Contents Logic�� Data�� ������ �ִ� Model ��ü
    /// </summary>
    public abstract class CommonWindow<TController> : CommonWindowBase
        where TController : WindowControllerBase
    {
        public override void SetController(Architecture.IController _controller)
        {
            if (!_controller is TController)
            {
                Debug.LogErrorFormat("Controller type must be {0}!! arg = {1}", typeof(TController), _controller.GetType());
                return;
            }
            base.SetController(_controller);
        }
        public TController GetController()
        {
            return this.Controller as TController;
        }
    }
}
