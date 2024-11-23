using System;
using System.Collections.Generic;
using Framework.Architecture;

namespace Framework.UI
{
    public sealed class UIManager : Singleton.Singleton<UIManager>, IUpdate, IPostLateUpdate
    {
        readonly List<WindowModelBase> openedWindows = new List<WindowModelBase>();
        readonly Dictionary<Type, WindowModelBase> loadedWindows = new Dictionary<Type, WindowModelBase>();
        readonly Queue<WindowModelBase> pendingCloseWindows = new Queue<WindowModelBase>();
        protected override void OnInit()
        {
            base.OnInit();
        }
        protected override void OnRelease()
        {
            base.OnRelease();
        }

        /// <summary>
        /// Load된 WindowBase를 얻어옵니다.
        /// </summary>
        /// <typeparam name="T">얻어올 WindowBase Type</typeparam>
        /// <param name="_load">true인 경우 Load 후 return</param>
        /// <returns>얻어온 WindowBase</returns>
        public T Get<T>(bool _load = false) where T : WindowBase
        {
            WindowModelBase _out = null;
            if(!loadedWindows.TryGetValue(typeof(T), out _out))
            {
                if (_load == false)
                    return null;

                _out = Load<T>();
            }

            return _out as T;
        }

        public T Load<T>() where T : WindowModelBase
        {
            WindowModelBase _out = null;
            if (!loadedWindows.TryGetValue(typeof(T), out _out))
            {
                _out = WindowModelBase.CreateWindow<T>();
                loadedWindows.Add(typeof(T), _out);
                _out.OnInit();
            }

            return _out as T;
        }
        /// <summary>
        /// 윈도우를 엽니다..
        /// </summary>
        /// <typeparam name="T">Open할 Window Type</typeparam>
        /// <returns>Open된 window model</returns>
        public T Open<T>(params object[] _params) where T : WindowBase
        {
            T _ret = Get<T>(true);
            if(!openedWindows.Contains(_ret))
            {
                openedWindows.Add(_ret);
            }
            _ret.OnOpen(_params);
            return _ret;
        }
        /// <summary>
        /// 윈도우를 닫는다.
        /// </summary>
        /// <typeparam name="T">닫을 윈도우 Type</typeparam>
        /// <param name="_destroy">true인경우 창을 삭제한다/param>
        public void Close<T>(bool _destroy = false) where T : WindowBase
        {
            T _ret = Get<T>();
            this.Close(_ret, _destroy);
        }
        /// <summary>
        /// CommonWindow를 엽니다. ( MessageBox등과 같은 다중으로 떠야 하는 윈도우류 )
        /// </summary>
        /// <typeparam name="T">Open할 CommonWindow Type</typeparam>
        /// <returns>Open�� ������</returns>
        public T OpenCommonWindow<T>(params object[] _params) where T : CommonWindowBase
        {
            T _ret = ObjectPool.Instance.Load<T>();
            if(_ret == null)
                _ret = WindowModelBase.CreateWindow<T>();

            _ret.OnInit();

            openedWindows.Add(_ret);
            _ret.OnOpen(_params);

            return _ret;
        }
        public void Close(WindowModelBase _target, bool _destroy = false)
        {
            openedWindows.Remove(_target);
            if (_target != null && !pendingCloseWindows.Contains(_target))
            {
                pendingCloseWindows.Enqueue(_target);
                _target.ReservedDestroy = _destroy;
            }
        } 
        public void OnUpdate(float _deltaTime)
        {
        }
        public void OnPostLateUpdate(float _deltaTime)
        {
            while(pendingCloseWindows.Count != 0)
            {
                WindowModelBase _window = pendingCloseWindows.Dequeue();
                _window.OnClose();

                if(_window.ReservedDestroy)
                {
                    loadedWindows.Remove(_window.GetType());
                    _window.OnDestroy();
                }
                else if(_window is CommonWindowBase)
                {
                    ObjectPool.Instance.Unload(_window as CommonWindowBase);
                }
            }            
        }
    }
}