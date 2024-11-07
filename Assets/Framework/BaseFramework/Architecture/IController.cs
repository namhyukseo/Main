using System;
using System.Collections;
using UnityEngine;
using MonoBehaviour = Framework.Architecture.MonoBehaviour;
namespace Framework.Architecture
{
    /// <summary>
    /// View와 Model을 잇는 Controller Interface
    /// GameObject에 Add되어있는 여타의 View객체를 Model과 연결하는 역할
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class IController : MonoBehaviour, ILateUpdate, IDisposable
    {
        private WeakReference<IModel> modelRef = new WeakReference<IModel>();
        protected IModel GetModel()
        {
            return this.modelRef.Target;
        }
        public virtual bool SetModel(IModel _model)
        {
            modelRef.Target = _model;
            if (_model != null)
            {
                _model.SetController(this);
            }
            return _model != null;
        }
        public bool RefreshDirtyFlag
        {
            private get; set;
        }
        protected abstract bool OnRefreshView();

        public void OnLateUpdate(float _deltaTime)
        {
            this.OnRefreshView();
        }
        protected abstract void Awake();
        protected abstract void Start();

        public void SetActive(bool _active)
        {
            this.gameObject.SetActive(_active);
        }

        public void Dispose()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}