using System;
using System.Collections;
using UnityEngine;

namespace Framework.Architecture
{
    public interface IUpdate
    {
        void OnUpdate(float _deltaTime);
    }
    public interface ILateUpdate
    {
        void OnLateUpdate(float _deltaTime);
    }
    public interface IPostLateUpdate
    {
        void OnPostLateUpdate(float _deltaTime);
    }
    public interface IFixedUpdate
    {
        void OnFixedUpdate(float _deltaTime);
    }    
    internal class RootMonoBehaviour : UnityEngine.MonoBehaviour
    {
        static GameObject rootGameObject = null;
        static RootMonoBehaviour rootMonoBehaviour = null;

        static public void Create()
        {
            rootGameObject = new GameObject("Root");
            GameObject.DontDestroyOnLoad(rootGameObject);
            rootGameObject.hideFlags = HideFlags.DontSave;
            rootMonoBehaviour = rootGameObject.AddComponent<RootMonoBehaviour>();
        }  

        public static Action<float> eventUpdate         = null;
        public static Action<float> eventLateUpdate     = null;
        public static Action<float> eventPostLateUpdate = null;
        public static Action<float> eventFixedUpdate    = null;
        public static Action<Event> eventGUIEvent       = null;

        private void Update()
        {
            eventUpdate?.Invoke(UnityEngine.Time.deltaTime);
        }
        private void LateUpdate()
        {
            eventLateUpdate?.Invoke(UnityEngine.Time.deltaTime);
            eventPostLateUpdate?.Invoke(UnityEngine.Time.deltaTime);
        }
        private void FixedUpdate()
        {
            eventFixedUpdate?.Invoke(UnityEngine.Time.fixedDeltaTime);
        }
        public static Coroutine Start_Coroutine(IEnumerator _routine)
        {
            return rootMonoBehaviour.StartCoroutine(_routine);
        }

        public static void Stop_Corutine(ref Coroutine _routine)
        {
            rootMonoBehaviour.StopCoroutine(_routine);
            _routine = null;
        }
    }

    /// <summary>
    /// MonoBehaviour의 Callback함수를 wrapping하기 위한 객체
    /// </summary>
    public abstract class MonoBehaviour : UnityEngine.MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            if (this is IUpdate)            RootMonoBehaviour.eventUpdate           += (this as IUpdate).OnUpdate;
            if (this is ILateUpdate)        RootMonoBehaviour.eventLateUpdate       += (this as ILateUpdate).OnLateUpdate;
            if (this is IPostLateUpdate)    RootMonoBehaviour.eventPostLateUpdate   += (this as IPostLateUpdate).OnPostLateUpdate;
            if (this is IFixedUpdate)       RootMonoBehaviour.eventFixedUpdate      += (this as IFixedUpdate).OnFixedUpdate;
        }

        protected virtual void OnDisable()
        {
            if (this is IUpdate)            RootMonoBehaviour.eventUpdate           -= (this as IUpdate).OnUpdate;
            if (this is ILateUpdate)        RootMonoBehaviour.eventLateUpdate       -= (this as ILateUpdate).OnLateUpdate;
            if (this is IPostLateUpdate)    RootMonoBehaviour.eventPostLateUpdate   -= (this as IPostLateUpdate).OnPostLateUpdate;
            if (this is IFixedUpdate)       RootMonoBehaviour.eventFixedUpdate      -= (this as IFixedUpdate).OnFixedUpdate;
        }
#if UNITY_EDITOR
        protected void Update()
        {
        }
        protected void LateUpdate()
        {
        }
        protected void FixedUpdate()
        {
        }
        protected void OnGUI()
        {
        }
#endif
    }
}