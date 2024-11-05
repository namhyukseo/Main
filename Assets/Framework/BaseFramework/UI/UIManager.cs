using System;
using System.Collections.Generic;

namespace Framework.UI
{
    public sealed class UIManager : Singleton.Singleton<UIManager> 
    {
        readonly List<WindowModelBase> openedWindows = new List<WindowModelBase>();
        readonly Dictionary<Type, WindowModelBase> loadedWindows = new Dictionary<Type, WindowModelBase>();

        protected override void OnInit()
        {
        }
        protected override void OnRelease()
        {
        }

        /// <summary>
        /// Load되어있는 WindowBase을 얻어온다.
        /// </summary>
        /// <typeparam name="T">생성하고자 하는 윈도우 타입</typeparam>
        /// <param name="_load">true인 경우 생성후 return</param>
        /// <returns>얻고자 하는 윈도우</returns>
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
        /// CommonWindow를 오픈한다. ( MessageBox등과 같이 다중으로 열릴 수 있는 윈도우류 )
        /// </summary>
        /// <typeparam name="T">열고자 하는 윈도우 타입</typeparam>
        /// <returns>Open된 윈도우</returns>

        public T OpenCommonWindow<T>(params object[] _params) where T : CommonWindowBase, new()
        {
            T _ret = ObjectPool.Instance.Load<T>();
            if(_ret == null)
                _ret = WindowModelBase.CreateWindow<T>();

            _ret.OnInit();

            openedWindows.Add(_ret);
            _ret.OnOpen(_params);
            return _ret;
        }
        /// <summary>
        /// Window를 오픈한다.
        /// </summary>
        /// <typeparam name="T">열고자 하는 윈도우 타입</typeparam>
        /// <returns>Open된 윈도우</returns>
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
        /// <typeparam name="T">닫고자 하는 윈도우 타입</typeparam>
        /// <param name="_destroy">true인경우 닫음과 동시에 삭제</param>
        public void Close<T>(bool _destroy = false) where T : WindowBase
        {
            T _ret = Get<T>();
            this.Close(_ret, _destroy);
        }

        public void Close(WindowModelBase _target, bool _destroy = false)
        {
            if (_target != null)
            {
                if (openedWindows.Contains(_target))
                {
                    _target.OnClose();
                    openedWindows.Remove(_target);
                }
                if (_destroy)
                {
                    _target.OnDestroy();
                    loadedWindows.Remove(_target.GetType());
                }
            }
        }
    }

}