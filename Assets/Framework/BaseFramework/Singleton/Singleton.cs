
using Framework.Architecture;
using System.Collections.Generic;

namespace Framework.Singleton
{
    public interface ISingleton
    {
        void Release();
    }
    public abstract class Singleton<T> : ISingleton  where T : class, ISingleton, new()
    {
        private static bool isActive = false;
        private static WeakReference<T> instance = null;

        static public bool IsActive { get { return isActive; } }
        static public T Instance
        {
            get
            {
                return instance.Target;
            }
        }

        protected Singleton()
        {
            this.OnInit();
            instance = new WeakReference<T>(this as T);
        }

        /// <summary>
        /// Singleton의 생성 시점에 호출되는 Callback함수,
        /// Singleton.Init를 통해 명시적으로 초기화가 호출되는 시점에 Singleton을 생성한 한 이후 SingletonContainer에 등록 후 호출.
        /// </summary>
        protected virtual void OnInit()
        {
            if(this is IUpdate)         RootMonoBehaviour.eventUpdate += (this as IUpdate).OnUpdate;
            if(this is ILateUpdate)     RootMonoBehaviour.eventLateUpdate += (this as ILateUpdate).OnLateUpdate;
            if(this is IPostLateUpdate) RootMonoBehaviour.eventPostLateUpdate += (this as IPostLateUpdate).OnPostLateUpdate;
        }

        /// <summary>
        /// Singleton을 해제 시키는 시점에 호출되는 Callback함수,
        /// Instance을 명시적으로 Release시키는 시점에 호출되며 이후 SingletonContainer에서 해제됨.
        /// </summary>
        protected virtual void OnRelease()
        {
            if(this is IUpdate)         RootMonoBehaviour.eventUpdate -= (this as IUpdate).OnUpdate;
            if(this is ILateUpdate)     RootMonoBehaviour.eventLateUpdate -= (this as ILateUpdate).OnLateUpdate;
            if(this is IPostLateUpdate) RootMonoBehaviour.eventPostLateUpdate -= (this as IPostLateUpdate).OnPostLateUpdate;
        }

        public static bool Create()
        {
            if (SingletonContainer.IsContains<T>())
            {
                UnityEngine.Debug.LogErrorFormat("[{0}은 이미 초기화 된 Singleton입니다.", typeof(T).ToString());
                return false;
            }

            return SingletonContainer.Add(new T());
        }

        public static bool Create<TController>() where TController : IController
        {
            if (SingletonContainer.IsContains<T>())
            {
                UnityEngine.Debug.LogErrorFormat("[{0}은 이미 초기화 된 Singleton입니다.", typeof(T).ToString());
                return false;
            }

            return SingletonContainer.Add(new T());
        }

        public void Release()
        {
            if(SingletonContainer.Remove(this as ISingleton))
            {
                this.OnRelease();
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("[{0}]은 이미 해제된 Singleton입니다.", this.GetType().ToString());
            }

            instance = null;
        }
    }

    public sealed class SingletonContainer
    {
        private static Dictionary<System.Type, ISingleton> container = new Dictionary<System.Type, ISingleton>();

        public static void Release()
        {
            /// 해제 되지 않은 등록된 모든 Singleton을 명시적으로 해제 시킨다.
            ISingleton[] _arrary = new ISingleton[container.Values.Count];
            container.Values.CopyTo(_arrary, 0);
            int _count = _arrary.Length;
            for (int _index = _count - 1; _index >= 0; --_index)
            {
                _arrary[_index].Release();
            }
            _arrary = null;
        }

        /// <summary>
        /// 해제되는 Singleton을 Singleton Container에서 해제한다.
        /// 직접 호출하지 말것!!
        /// </summary>
        /// <param name="_instance">제거할 대상</param>
        /// <returns>제거 성공여부</returns>
        public static bool Remove(ISingleton _instance)
        {
            return container.Remove(_instance.GetType());
        }
        /// <summary>
        /// 새로 생선된 Singleton을 Singleton Container에 등록한다.
        /// 직접 호출하지 말것!!
        /// </summary>
        /// <param name="_instance">등록할 대상</param>
        /// <returns>등록 성공여부</returns>
        public static bool Add(ISingleton _instance)
        {
            if(container.ContainsKey(_instance.GetType()))
            {
                return false;
            }
            container.Add(_instance.GetType(), _instance);
            return true;
        }
        /// <summary>
        /// 이미 생성된 Singleton이 존재하는지 체크한다.
        /// </summary>
        /// <param name="_instance">검색할 대상</param>
        /// <returns>등록 성공여부</returns>
        public static bool IsContains<T>() where T : ISingleton
        {
            return container.ContainsKey(typeof(T));
        }
    }
}